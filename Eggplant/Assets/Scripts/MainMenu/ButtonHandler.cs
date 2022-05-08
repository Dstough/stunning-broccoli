using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public GameObject Server;
    public GameObject Client;

    public void OnHostClick()
    {
        Instantiate(Server, new Vector3(0, 0, 0), Quaternion.identity);

        SceneManager.LoadScene("Chat Room");
    }

    public void OnJoinClick()
    {
        Instantiate(Client, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
