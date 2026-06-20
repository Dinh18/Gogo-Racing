using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerInputController playerInput = collision.gameObject.GetComponent<PlayerInputController>();
            playerInput.isStop = true;
            playerInput.ResetInput();


            // CarMovement carMovement = collision.gameObject.GetComponent<CarMovement>();
            // carMovement.CarBroke();
        }
        // if(collision.gameObject.CompareTag("NPC"))
        // {
        //     AIInputController aIInputController = collision.gameObject.GetComponent<AIInputController>();
        //     aIInputController.isStop = true;
        //     // aIInputController.ResetInput();
        // }
    }
}
