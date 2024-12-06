using UnityEngine;
using System.Collections;
using UnityEngine.Android;
using TMPro;

public class LocationManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt_Latitude;
    [SerializeField] TextMeshProUGUI txt_Longitude;
    [SerializeField] TextMeshProUGUI txt_Altitude;
    [SerializeField] TextMeshProUGUI txt_Accuracy;

    void Start()
    {
        GetMyLocation();
    }

    public void GetMyLocation()
    {
        StartCoroutine(CheckAndRequestPermission());
    }
     
    IEnumerator CheckAndRequestPermission()
    {
        // ���� Ȯ��
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            // ���� ��û �� ��� ���
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null; // ���� �����ӱ��� ���
            }
        }

        // ������ ���� ��� ��ġ �������� �Լ� ����
        StartCoroutine(GetLocation());
    }

    IEnumerator GetLocation()
    {
        // ��ġ ���� Ȱ��ȭ ���� Ȯ��
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("========== LOCATION SERVICE DISABLED ==========");
            OpenLocationSettings();
            yield break;
        }

        // ��ġ ���� ����
        Input.location.Start();

        // �ʱ�ȭ ���
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // �ʱ�ȭ ���� ó��
        if (maxWait <= 0)
        {
            Debug.Log("========== LOCATION SERVICE INITIALIZATION TIMEOUT ==========");
            yield break;
        }

        // ��ġ ���� ���� ó��
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("========== FAILED TO GET LOCATION ==========");
            yield break;
        }
        else
        {
            // ��ġ ������ �α� ���
            Debug.Log("========== LOCATION DATA ==========");
            Debug.Log($"Latitude: {Input.location.lastData.latitude}");
            Debug.Log($"Longitude: {Input.location.lastData.longitude}");
            Debug.Log($"Altitude: {Input.location.lastData.altitude}");
            Debug.Log($"Accuracy: {Input.location.lastData.horizontalAccuracy}");

            txt_Latitude.text = $"Latitude: {Input.location.lastData.latitude}";
            txt_Longitude.text = $"Longitude: {Input.location.lastData.longitude}";
            txt_Altitude.text = $"Altitude: {Input.location.lastData.altitude}";
            txt_Accuracy.text = $"Accuracy: {Input.location.lastData.horizontalAccuracy}";
            Debug.Log("========== END OF LOCATION DATA ==========");

        }

        // ��ġ ���� ����
        Input.location.Stop();
    }

    void OpenLocationSettings()
    {
        Debug.Log("Opening location settings...");
        Application.OpenURL("package:" + Application.identifier); // Android �������� �̵�
    }
}
