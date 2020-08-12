using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class IpAddressFinder : MonoBehaviour
{
    public Text lblExternalIp;

    void Start()
    {
        lblExternalIp.text = new WebClient().DownloadString("http://icanhazip.com").Trim();
    }
}
