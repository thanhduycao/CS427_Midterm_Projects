using UnityEngine;

public class DialogZone : MonoBehaviour
{
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private GameObject canvasGameObject;
    [SerializeField] private GameObject dialog;
    [SerializeField] private GameObject dialogLine;
    private bool isInZone = false;
    private MovementNoMana playerMovement;
    void Update()
    {
        bool isDialogProcessing = dialogLine.GetComponent<DialogSystem.DialogLine>().isProcessing;
        if (isInZone && isDialogProcessing == false)
        {
            if (playerMovement.isDialogProcessing == true)
            {
                playerMovement.isDialogProcessing = false;
            }
            if (Input.GetKey(KeyCode.P))
            {
                dialogLine.GetComponent<DialogSystem.DialogLine>().jsonFile = jsonFile;
                dialog.SetActive(true);
                playerMovement.isDialogProcessing = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _))
        {
            canvasGameObject.SetActive(true);
            isInZone = true;
            playerMovement = collision.gameObject.GetComponent<MovementNoMana>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _))
        {
            canvasGameObject.SetActive(false);
            isInZone = false;
        }
    }
}
