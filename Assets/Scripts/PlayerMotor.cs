using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 rotate;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterVelocity;
    private Rigidbody rb;

    [SerializeField]
    private Camera cam;

    //On limite l'axe de rotation vertical de la caméra à 85°
    [SerializeField]
    private float cameraRotationLimit = 85f;


    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity){
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotate){
        rotate = _rotate;
    }

    public void RotateCamera(float _cameraRotationX){
        cameraRotationX = _cameraRotationX;
    }

    public void ApplyThruster(Vector3 _thrustervelocity){
        thrusterVelocity = _thrustervelocity;
    }

    private void FixedUpdate() {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement(){
        //Si vector3.zero, c'est qu'on n'appuie sur aucune touche de mouvement
        if(velocity != Vector3.zero){
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        //Déplacement vers le haut du jetpack
        if(thrusterVelocity != Vector3.zero){
            rb.AddForce(thrusterVelocity * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void PerformRotation(){
        //Quaternion.Euler permet de transformer notre rotation en Quaternion
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotate));
        //On met un moins car sinon inverse la direction de la caméra par rapport au mouvement souris
        currentCameraRotationX -= cameraRotationX;
        //Clamp permet de limiter currentCameraRotationX entre une valeur min et max définie en param
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        //On applique la rotation de la caméra
        cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

}
