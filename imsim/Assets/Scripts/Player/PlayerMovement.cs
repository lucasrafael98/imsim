using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public Camera cam;
    public Transform mantleCheck;
    public LayerMask groundMask;

    public float mantleDistance = 0.2f;
    public float crouchSpeed = 2f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;
    public float leanAngle = 20f;
    public float leanSpeed = 100f;
    public float headBobSpeed = 5;
    public float headBobIntensity = 0.0001f;

    float rotX, rotZ, hBobZ;
    float camY_standing, camY_crouching, collH_standing,
            collH_crouching, collY_standing, collY_crouching;
    float jumpZ, jumpSpeed;
    float ledgeY, cumMantleFwd;

    float crouchFactor = 0.7f;
    bool crouching, sprinting, crouched, jumping, mantling;
    Vector3 velocity;

    private CharacterController ctrl;
    private CapsuleCollider coll;

    void Start() {
        ctrl = transform.GetComponent<CharacterController>();
        coll = transform.GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camY_standing = cam.transform.localPosition.y;
        camY_crouching = camY_standing * crouchFactor;
        collH_standing = ctrl.height;
        collH_crouching = ctrl.height * crouchFactor;
        collY_standing = -0.05f;
        collY_crouching = -crouchFactor / 2f;
    }

    void Update() {
        float speed = walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        Quaternion adjust = adjustMoveToSlope(move);
        move = adjust * move;
        float mouseX = Cursor.visible ? 0 : Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        handleCameraMove();

        if (Input.GetKey("space") && !mantling) {
            checkMantle();
        }

        if (mantling) {
            handleMantleMove();
            return;
        }

        if (ctrl.isGrounded) {
            if (velocity.y <= 0) {
                jumping = false;
                velocity.y = 0f;
            }
            if (Input.GetKeyDown("space") && !jumping) {
                velocity.y += 8f;
                jumpZ = z;
                jumpSpeed = sprinting ? runSpeed * 1.2f : walkSpeed;
                jumping = true;
            }
            if (Input.GetKeyDown("c")) {
                crouching = !crouching;
                crouched = false;
                hBobZ = 0f;
            }
        }

        if (Input.GetKey("left shift") && ctrl.isGrounded && !crouching) {
            speed = runSpeed;
            sprinting = true;
        } else {
            sprinting = false;
        }

        if (crouching) {
            speed = crouchSpeed;
            if (coll.height > collH_crouching) {
                coll.height -= Time.deltaTime * 4;
                ctrl.height -= Time.deltaTime * 4;
            }
            if (coll.center.y > collY_crouching) {
                coll.center = new Vector3(0f, coll.center.y - Time.deltaTime * 2, 0f);
                ctrl.center = new Vector3(0f, ctrl.center.y - Time.deltaTime * 2, 0f);
            }
            if (cam.transform.localPosition.y > camY_crouching && !crouched)
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y - Time.deltaTime * 4, cam.transform.localPosition.z);
            else if ((x != 0 || z != 0) && ctrl.isGrounded) {
                crouched = true;
                hBobZ += Time.deltaTime * headBobSpeed / 2;
                hBobZ %= Mathf.PI + 0.9f;
                float new_camY = camY_crouching + ((hBobZ > Mathf.PI + 0.6f) ? Mathf.Sin(Mathf.PI + 0.4f) : Mathf.Sin(hBobZ - 0.4f)) * headBobIntensity * 200;
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, new_camY, cam.transform.localPosition.z);
            }
        } else {
            if (coll.height < collH_standing) {
                coll.height += Time.deltaTime * 4;
                ctrl.height += Time.deltaTime * 4;
            }
            if (coll.center.y < collY_standing) {
                coll.center = new Vector3(0f, coll.center.y + Time.deltaTime * 2, 0f);
                ctrl.center = new Vector3(0f, ctrl.center.y + Time.deltaTime * 2, 0f);
            }
            if (cam.transform.localPosition.y < camY_standing)
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y + Time.deltaTime * 4, cam.transform.localPosition.z);
            else if ((x != 0 || z != 0) && ctrl.isGrounded) {
                hBobZ += sprinting ? Time.deltaTime * headBobSpeed * 2 : Time.deltaTime * headBobSpeed;
                hBobZ %= 2 * Mathf.PI;
                float new_camY = cam.transform.localPosition.y + Mathf.Sin(hBobZ) * headBobIntensity;
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, new_camY, cam.transform.localPosition.z);
            }
        }

        if (Input.GetKey("q") && rotZ <= leanAngle) {
            rotZ += Time.deltaTime * leanSpeed;
            if (cam.transform.localPosition.x > -1)
                cam.transform.localPosition -= new Vector3(1, 0, 0) * Time.deltaTime * leanSpeed / 20;
        } else if (Input.GetKey("e") && rotZ >= -leanAngle) {
            rotZ -= Time.deltaTime * leanSpeed;
            if (cam.transform.localPosition.x < 1)
                cam.transform.localPosition += new Vector3(1, 0, 0) * Time.deltaTime * leanSpeed / 20;
        } else {
            if (rotZ > 1) {
                rotZ -= Time.deltaTime * leanSpeed;
                if (cam.transform.localPosition.x < 1)
                    cam.transform.localPosition += new Vector3(1, 0, 0) * Time.deltaTime * leanSpeed / 20;
            } else if (rotZ < -1) {
                rotZ += Time.deltaTime * leanSpeed;
                if (cam.transform.localPosition.x > -1)
                    cam.transform.localPosition -= new Vector3(1, 0, 0) * Time.deltaTime * leanSpeed / 20;
            } else
                rotZ = 0;
        }
        rotZ = Mathf.Clamp(rotZ, -leanAngle, leanAngle);

        velocity.y += gravity * Time.deltaTime;

        if (!jumping) ctrl.Move(move * speed * Time.deltaTime);
        else {
            move = transform.right * (x / 2) + transform.forward * jumpZ;
            ctrl.Move(move * jumpSpeed * Time.deltaTime);
        }
        ctrl.Move(velocity * Time.deltaTime);
        transform.Rotate(Vector3.up * mouseX);
    }

    private Quaternion adjustMoveToSlope(Vector3 move) {
        Quaternion q = new Quaternion();
        Vector3 pos = transform.position;
        pos.y = coll.bounds.min.y;
        if (Physics.Raycast(pos, -transform.up, out RaycastHit hit, ctrl.stepOffset)) {
            q = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        return q;
    }

    private void checkMantle() {
        if (Physics.Raycast(mantleCheck.position, transform.forward, out RaycastHit hit, mantleDistance)) {
            for (float i = 0.01f; i <= 0.2f; i += 0.01f) {
                Vector3 pos = mantleCheck.position;
                pos.y += i;
                if (!Physics.Raycast(pos, transform.forward, out hit, mantleDistance)) {
                    ledgeY = mantleCheck.position.y + i;
                    jumping = false; mantling = true;
                }
            }
        }
    }

    private void handleMantleMove() {
        if (coll.bounds.min.y < ledgeY) { // climbing
            ctrl.Move(transform.up * Time.deltaTime * 4);
        } else if (!ctrl.isGrounded) { // moving fwd slightly
            ctrl.Move(transform.forward * Time.deltaTime * 4);
            cumMantleFwd += Time.deltaTime * 4;
        }

        if (cumMantleFwd > coll.bounds.max.x - coll.bounds.min.x || ctrl.isGrounded) {
            cumMantleFwd = 0; mantling = false;
        }
    }

    private void handleCameraMove() {
        float mouseY = Cursor.visible ? 0 : Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(rotX, 0f, rotZ);
    }
}
