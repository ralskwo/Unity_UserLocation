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
        // 권한 확인
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            // 권한 요청 후 결과 대기
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null; // 다음 프레임까지 대기
            }
        }

        // 권한이 허용된 경우 위치 가져오기 함수 실행
        StartCoroutine(GetLocation());
    }

    IEnumerator GetLocation()
    {
        // 위치 서비스 활성화 여부 확인
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("========== LOCATION SERVICE DISABLED ==========");
            OpenLocationSettings();
            yield break;
        }

        // 위치 서비스 시작
        Input.location.Start();

        // 초기화 대기
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 초기화 실패 처리
        if (maxWait <= 0)
        {
            Debug.Log("========== LOCATION SERVICE INITIALIZATION TIMEOUT ==========");
            yield break;
        }

        // 위치 서비스 실패 처리
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("========== FAILED TO GET LOCATION ==========");
            yield break;
        }
        else
        {
            // 위치 데이터 로그 출력
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

        // 위치 서비스 종료
        Input.location.Stop();
    }

    void OpenLocationSettings()
    {
        Debug.Log("Opening location settings...");
        Application.OpenURL("package:" + Application.identifier); // Android 설정으로 이동
    }
}
