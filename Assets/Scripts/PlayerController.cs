using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    //Vitesse du joueur
    private float speed = 3f;

    [SerializeField]
    //Vitesse de la souris pour bouger la caméra
    private float mouseSensitivity = 3f;

    [SerializeField]
    //Puissance du jetpack
    private float thrusterForce = 1000f;

    [Header("Paramètre joint")]
    [SerializeField]
    private float jointSpring = 20f;

    [SerializeField]
    private float jointMaxForce = 150f;

    private PlayerMotor playerMotor;
    private ConfigurableJoint configurableJoint;

    private void Start() {
        playerMotor = GetComponent<PlayerMotor>();
        configurableJoint = GetComponent<ConfigurableJoint>();
        //On initialise la gravité et l'effet de rebondissement
        SetJointSettings(jointSpring);
    }

    private void Update() {
        //calcul de la vitesse du joueur
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * yMove;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;       

        playerMotor.Move(velocity); 


        //On calcule la rotation horizontale du joueur
        float yRot = Input.GetAxisRaw("Mouse X");
        
        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivity;

        playerMotor.Rotate(rotation);


        //On calcule la rotation verticale de la caméra
        float xRot = Input.GetAxisRaw("Mouse Y");
        
        float cameraRotationX = xRot * mouseSensitivity;

        playerMotor.RotateCamera(cameraRotationX);


        //gestion du jetpack (touche espace)
        Vector3 thrusterVelocity = Vector3.zero;
        if(Input.GetButton("Jump")){
            //Tant que l'on utilise le jetpack, on désactive la gravité et le rebondissement
            SetJointSettings(0f);
            thrusterVelocity = Vector3.up * thrusterForce;
        }else{
            //on remet la gravité et le rebondissement
            SetJointSettings(jointSpring);
        }

        playerMotor.ApplyThruster(thrusterVelocity);
    }

    //Lorsque l'on utilise plus le jetpack, on remet les valeurs du joint
    private void SetJointSettings(float _jointSpring){
        configurableJoint.yDrive = new JointDrive{positionSpring = _jointSpring, maximumForce = jointMaxForce};
    }


}
