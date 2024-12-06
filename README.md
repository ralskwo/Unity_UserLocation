# Unity Android GPS: 사용자 위치 및 권한 요청

이 프로젝트는 Unity를 사용하여 Android에서 GPS 기능을 구현하는 방법을 보여줍니다. 사용자 위치(위도, 경도, 고도, 정확도)를 가져오는 방법과 Android 런타임 권한 요청 처리, 그리고 TextMeshPro를 사용하여 위치 데이터를 화면에 표시하는 과정을 포함합니다.

---

## 주요 기능

- **런타임 권한 요청**: Android 디바이스에서 위치 권한을 런타임에 요청 및 확인.
- **GPS 데이터 가져오기**: 위도, 경도, 고도, 정확도를 가져와 실시간으로 표시.
- **안드로이드 위치 서비스 처리**: 위치 서비스가 비활성화된 경우 사용자에게 이를 활성화하도록 안내.
- **TextMeshPro 통합**: Unity의 TextMeshPro를 사용하여 위치 데이터를 UI에 출력.

---

## 요구사항

- **Unity 버전**: Unity 2022.3.x 이상 (TextMeshPro가 기본 포함된 버전).
- **Android 디바이스**: GPS 및 위치 서비스가 활성화된 상태에서 테스트 필요.
- **Android SDK 설정**: 최소 API 레벨 23(Android 6.0) 이상.

---

## 사용 방법

### 1. Unity 프로젝트 설정

1. Unity에서 Android 플랫폼을 선택:
   - `File > Build Settings > Android`에서 플랫폼을 Android로 변경.
2. **Player Settings** 설정:
   - `Other Settings`에서 **Minimum API Level**을 `Android 6.0(API 23)` 이상으로 설정.
   - **Target API Level**은 최신 Android 버전으로 설정 권장.

### 2. Android 권한 추가

`AndroidManifest.xml` 파일에 다음 권한을 추가합니다:

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
```

#### 설명

- ACCESS_FINE_LOCATION: 고정밀 GPS 데이터를 가져오기 위한 권한.
- ACCESS_COARSE_LOCATION: Wi-Fi 또는 셀룰러 네트워크 기반의 대략적인 위치 데이터 접근.
- ACCESS_BACKGROUND_LOCATION: 앱이 백그라운드 상태에서도 위치 데이터를 가져오기 위해 필요한 권한.
  - Android 10(API 29) 이상에서 필수.
  - Google Play 정책에 따라 사용 목적을 명확히 해야 하며, 백그라운드 권한은 앱 리뷰 심사가 필요할 수 있습니다.

### 3. TextMeshPro 설정

1. **TextMeshPro 패키지 설치**:

   - Unity 상단 메뉴에서 `Window > Package Manager`를 선택합니다.
   - 목록에서 **TextMeshPro**를 검색한 후 설치합니다.

2. **TextMeshPro 컴포넌트 추가**:

   - `Hierarchy`에서 UI Canvas를 생성하거나 기존 Canvas를 선택합니다.
   - `Right Click > UI > Text - TextMeshPro`를 선택하여 TextMeshPro UI 요소를 추가합니다.

3. **TextMeshPro 변수 연결**:
   - 생성된 TextMeshPro 텍스트 객체를 **txt_Latitude**, **txt_Longitude**, **txt_Altitude**, **txt_Accuracy** 변수에 연결합니다.
   - 이를 위해 `Inspector` 창에서 LocationManager 스크립트를 포함한 GameObject를 선택한 후, 각 TextMeshPro 객체를 `txt_Latitude`, `txt_Longitude`, `txt_Altitude`, `txt_Accuracy` 필드에 드래그하여 연결합니다.

### 4. 프로젝트 실행

1. **Android 디바이스 빌드**:

   - Unity에서 `File > Build Settings`로 이동합니다.
   - 플랫폼을 **Android**로 설정한 후 `Switch Platform`을 클릭합니다.
   - 디바이스에서 앱이 정상적으로 동작하도록 설정을 확인한 후 `Build and Run`으로 디바이스에 설치합니다.

2. **앱 실행 및 권한 허용**:

   - Android 디바이스에서 앱을 실행하면 **GPS 권한 요청 팝업**이 표시됩니다.
   - "허용"을 선택하면 위치 서비스가 활성화되고 GPS 데이터를 가져옵니다.

3. **TextMeshPro UI에 위치 데이터 표시**:
   - 앱이 실행되면 사용자의 **위도**, **경도**, **고도**, **정확도** 데이터가 TextMeshPro UI를 통해 실시간으로 표시됩니다.
   - 사용자가 이동하거나 GPS 신호가 업데이트되면 데이터가 변경됩니다.

---

## 주요 코드

### 1. 권한 요청 및 위치 데이터 가져오기

- Android 런타임 권한을 확인하고 요청합니다.
- 사용자가 권한을 허용할 때까지 대기하며, 권한이 허용되면 위치 데이터를 가져오는 코루틴을 실행합니다.

```csharp
if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
{
    Permission.RequestUserPermission(Permission.FineLocation);

    while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
    {
        yield return null;
    }
}

StartCoroutine(GetLocation());
```

### 2. 위치 데이터 UI 출력

- 위도(Latitude), 경도(Longitude), 고도(Altitude), 정확도(Accuracy)를 TextMeshPro UI에 표시합니다.
- 사용자의 위치 정보는 실시간으로 업데이트되며, UI를 통해 명확하게 데이터를 확인할 수 있습니다.

```csharp
txt_Latitude.text = $"Latitude: {Input.location.lastData.latitude}";
txt_Longitude.text = $"Longitude: {Input.location.lastData.longitude}";
txt_Altitude.text = $"Altitude: {Input.location.lastData.altitude}";
txt_Accuracy.text = $"Accuracy: {Input.location.lastData.horizontalAccuracy}";
```

### 3. 위치 서비스 비활성화 처리

- 위치 서비스가 비활성화된 경우, Android 설정 화면으로 사용자를 유도합니다.
- 이 기능은 GPS가 꺼져 있거나 디바이스의 위치 서비스가 비활성화된 경우 앱이 올바르게 동작하도록 보장합니다.

```csharp
if (!Input.location.isEnabledByUser)
{
    Debug.Log("========== LOCATION SERVICE DISABLED ==========");
    Application.OpenURL("package:" + Application.identifier); // Android 설정으로 이동
    yield break;
}
```

---

## 참고 사항

### Unity 에디터 테스트 제한

- Unity 에디터에서는 GPS 기능이 지원되지 않습니다. Android 디바이스에서 직접 테스트해야 합니다.

### 권한 요청

- Android 6.0(API 23) 이상에서는 런타임 권한 요청이 필수입니다. 사용자가 권한 요청을 거부한 경우 설정 화면으로 유도해야 합니다.

### Google Play 정책 준수

- Google Play 스토어에 앱을 업로드할 경우, 위치 권한 사용 목적을 명확히 기재해야 합니다.
- 백그라운드 권한을 사용하는 경우 추가적인 정책 심사를 통과해야 할 수 있습니다.

---

## 향후 확장 가능성

이 코드를 기반으로 다음과 같은 기능을 추가하거나 확장할 수 있습니다:

- **Google Maps API 통합**: 지도를 활용한 고급 위치 기반 서비스를 개발할 수 있습니다.
- **AR 콘텐츠 결합**: 증강현실(AR)과 GPS 데이터를 결합하여 몰입형 경험을 제공합니다.
- **위치 기반 게임 및 앱 개발**: 특정 위치에서만 접근 가능한 콘텐츠나 이벤트를 포함한 위치 기반 앱을 개발할 수 있습니다.

## 라이선스

이 프로젝트는 [MIT License](https://opensource.org/licenses/MIT)에 따라 배포됩니다.
