using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ARTrackedImg : MonoBehaviour
{

    // ARTrackedImageManager를 통해 AR 이미지 추적 관리
    public ARTrackedImageManager trackedImageManager;

    // 3D 오브젝트 리스트
    public List<GameObject> _objectList = new List<GameObject>();

    public TextMeshProUGUI objectDescriptionText; // 오브젝트 설명을 표시할 UI 텍스트

    // 이미지 이름을 키로, 해당 이미지를 추적할 때 표시할 3D 오브젝트를 값으로 가지는 딕셔너리
    private Dictionary<string, GameObject> _prefabDic = new Dictionary<string, GameObject>();

    // 설명 텍스트를 미리 설정
    private Dictionary<string, string> _objectDescriptions = new Dictionary<string, string>()
    {
        {"gyojeon", "왜검교전은 일본에서 도입된 칼류의 도검무예이다. 두 사람이 마주 보고 칼을 사용하여 교전하는 형식으로 공방의 자세를 취하는 것이 특징이다."},
        {"deungpae", "등패는 중국에서 도입된 칼류의 도검무예이다. 등패와 함께 짧은 칼인 요도와 표창으로 무장하여, 먼저 표창을 던지고 이후 등패와 칼로 적을 제압하는 근접전을 펼친다."},
        {"ssanggeom", "쌍검은 중국에서 도입된 칼류의 도검무예이다. 두 개의 칼을 사용하는 기예로서 하나로는 막고 다른 칼로 상대를 베거나 찌른다."},
        {"ssangsudo", "쌍수도는 일본에서 도입된 칼류의 도검무예이다. 모든 검법의 기본으로서 검의 운용에 필요한 발도술, 베기, 막기, 찌르기, 착검술, 안법, 보법 등으로 이루어져 있다."},
        {"nangseon", "낭선은 중국에서 도입된 창류의 단병무예이다. 낭선은 대나무 가지에 작은 쇠 날을 9개~11개 달아 만든 무기로 찌르는 형태의 자법을 사용하고 있다."},
        {"dangpa", "당파는 중국에서 도입된 창류의 무기이다. 정봉이라고 불리는 가운데의 긴 날과 양쪽으로 두 개의 가지가 뻗은 모양을 갖고 있다."},
        {"woldo", "월도는 중국에서 도입된 칼류의 도검무예이다. 도의 형태로 찍어 베는 감법을 사용하고 있다. 실전에서 쓰는 군사훈련용보다는 의례에서 행해지는 웅장함을 드러낼 때 사용하는 도검무예이다."},
        {"hyeopdo", "협도는 중국에서 도입된 칼류의 도검무예이다. 장검처럼 눈썹 모양을 하고 도를 이용하여 찍어 베는 형태의 감법을 사용하고 있다."},
        {"gwonbeop", "권법은 손과 발을 이용하여 상대방을 공격하거나 방어하는 맨손 무예이다. 무예를 배우는 자가 가장 먼저 익히는 기초적인 무예로서 주먹으로 치는 형태의 격법을 사용하고 있다."},
    };

    void Awake()
    {
        // 오브젝트 리스트에 있는 3D 오브젝트를 이름과 함께 딕셔너리에 추가
        foreach (GameObject obj in _objectList)
        {
            string tName = obj.name;  // 각 오브젝트의 이름을 가져옴
            _prefabDic.Add(tName, obj);  // 오브젝트 이름을 키로 딕셔너리에 저장 (추적된 이미지와 매핑할 준비)
        }
    }

    private void OnEnable()
    {
        // 이미지가 추적되거나 업데이트될 때 호출되는 이벤트 핸들러 등록
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        // 컴포넌트가 비활성화되면 이벤트 핸들러 해제
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    // 이미지가 새로 인식되었거나 업데이트된 경우에 호출됨
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 새로 추가된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);  // 이미지에 해당하는 오브젝트를 배치
        }

        // 업데이트된 이미지 처리 (기존 이미지가 움직이거나 상태가 변경된 경우)
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);  // 이미지에 대응하는 오브젝트의 위치 및 회전을 업데이트
        }

        // 추적이 중지된 이미지 처리 (이미지가 보이지 않을 때)
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            DisableImage(trackedImage);  // 오브젝트 비활성화
        }
    }

    // 추적된 이미지에 대응하는 오브젝트의 위치와 회전을 업데이트하고, 오브젝트를 활성화
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;  // 추적된 이미지의 이름을 가져옴
        GameObject tObj = _prefabDic[name];  // 해당 이름에 매핑된 오브젝트를 딕셔너리에서 가져옴
        tObj.transform.position = trackedImage.transform.position;  // 이미지가 있는 위치에 오브젝트 위치를 맞춤
        tObj.transform.rotation = trackedImage.transform.rotation;  // 이미지의 회전에 맞춰 오브젝트 회전을 설정
        tObj.SetActive(true);  // 오브젝트를 활성화하여 씬에 표시

        // 오브젝트에 대한 설명을 UI에 표시
        if (_objectDescriptions.ContainsKey(name))
        {
            objectDescriptionText.text = _objectDescriptions[name]; // 설명 텍스트 업데이트
            objectDescriptionText.gameObject.SetActive(true); // 설명 UI 표시
        }
    }

    // 추적이 중지된 이미지에 대응하는 오브젝트를 비활성화
    private void DisableImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;  // 추적된 이미지의 이름을 가져옴
        if (_prefabDic.ContainsKey(name))
        {
            GameObject tObj = _prefabDic[name];  // 해당 이름에 매핑된 오브젝트를 딕셔너리에서 가져옴
            tObj.SetActive(false);  // 오브젝트를 비활성화
            objectDescriptionText.gameObject.SetActive(false);  // UI 텍스트도 비활성화
        }
    }
}
