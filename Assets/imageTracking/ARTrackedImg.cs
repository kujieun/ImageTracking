using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ARTrackedImg : MonoBehaviour
{

    // ARTrackedImageManager를 통해 AR 이미지 추적 관리
    public ARTrackedImageManager trackedImageManager;

    // 3D 오브젝트 리스트
    public List<GameObject> _objectList = new List<GameObject>();

    // 이미지 이름을 키로, 해당 이미지를 추적할 때 표시할 3D 오브젝트를 값으로 가지는 딕셔너리
    private Dictionary<string, GameObject> _prefabDic = new Dictionary<string, GameObject>();

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
    }

    // 추적이 중지된 이미지에 대응하는 오브젝트를 비활성화
    private void DisableImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;  // 추적된 이미지의 이름을 가져옴
        if (_prefabDic.ContainsKey(name))
        {
            GameObject tObj = _prefabDic[name];  // 해당 이름에 매핑된 오브젝트를 딕셔너리에서 가져옴
            tObj.SetActive(false);  // 오브젝트를 비활성화
        }
    }
}
