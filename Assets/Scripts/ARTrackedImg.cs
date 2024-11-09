using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using System.Net;
using System.Text;
using System.IO;
using UnityEngine.Networking;


public class ARTrackedImg : MonoBehaviour
{
    // ARTrackedImageManager�� ���� AR �̹��� ���� ����
    public ARTrackedImageManager trackedImageManager;

    // 3D ������Ʈ ����Ʈ
    public List<GameObject> _objectList = new List<GameObject>();

    public TextMeshProUGUI objectDescriptionText; // ������Ʈ ������ ǥ���� UI �ؽ�Ʈ

    public NaverTTS tts; // NaverTTS ��ũ��Ʈ�� ����

    // �̹��� �̸��� Ű��, �ش� �̹����� ������ �� ǥ���� 3D ������Ʈ�� ������ ������ ��ųʸ�
    private Dictionary<string, GameObject> _prefabDic = new Dictionary<string, GameObject>();

    // ���� �ؽ�Ʈ�� �̸� ����
    private Dictionary<string, string> _objectDescriptions = new Dictionary<string, string>()
    {
        {"gyojeon", "�ְ˱����� �Ϻ����� ���Ե� Į���� ���˹����̴�. �� ����� ���� ���� Į�� ����Ͽ� �����ϴ� �������� ������ �ڼ��� ���ϴ� ���� Ư¡�̴�."},
        {"deungpae", "���д� �߱����� ���Ե� Į���� ���˹����̴�. ���п� �Բ� ª�� Į�� �䵵�� ǥâ���� �����Ͽ�, ���� ǥâ�� ������ ���� ���п� Į�� ���� �����ϴ� �������� ��ģ��."},
        {"ssanggeom", "�ְ��� �߱����� ���Ե� Į���� ���˹����̴�. �� ���� Į�� ����ϴ� �⿹�μ� �ϳ��δ� ���� �ٸ� Į�� ��븦 ���ų� ���."},
        {"ssangsudo", "�ּ����� �Ϻ����� ���Ե� Į���� ���˹����̴�. ��� �˹��� �⺻���μ� ���� ��뿡 �ʿ��� �ߵ���, ����, ����, ���, ���˼�, �ȹ�, ���� ������ �̷���� �ִ�."},
        {"nangseon", "������ �߱����� ���Ե� â���� �ܺ������̴�. ������ �볪�� ������ ���� �� ���� 9��~11�� �޾� ���� ����� ��� ������ �ڹ��� ����ϰ� �ִ�."},
        {"dangpa", "���Ĵ� �߱����� ���Ե� â���� �����̴�. �����̶�� �Ҹ��� ����� �� ���� �������� �� ���� ������ ���� ����� ���� �ִ�."},
        {"woldo", "������ �߱����� ���Ե� Į���� ���˹����̴�. ���� ���·� ��� ���� ������ ����ϰ� �ִ�. �������� ���� �����Ʒÿ뺸�ٴ� �Ƿʿ��� �������� �������� �巯�� �� ����ϴ� ���˹����̴�."},
        {"hyeopdo", "������ �߱����� ���Ե� Į���� ���˹����̴�. ���ó�� ���� ����� �ϰ� ���� �̿��Ͽ� ��� ���� ������ ������ ����ϰ� �ִ�."},
        {"gwonbeop", "�ǹ��� �հ� ���� �̿��Ͽ� ������ �����ϰų� ����ϴ� �Ǽ� �����̴�. ������ ���� �ڰ� ���� ���� ������ �������� �����μ� �ָ����� ġ�� ������ �ݹ��� ����ϰ� �ִ�."},
    };

    private float initialDistance;
    private float previousTouchDistance = 0; // ���� �հ��� �Ÿ�
    private Vector3 initialScale;
    private Quaternion initialRotation; // �ʱ� ȸ���� ����
    public GameObject ObjectPool; // ������Ʈ�� ��� �ִ� �θ� ������Ʈ
    private float accumulatedRotation = 0f;
    private Vector2 initialTouchPosition;

    void Awake()
    {
        // ������Ʈ ����Ʈ�� �ִ� 3D ������Ʈ�� �̸��� �Բ� ��ųʸ��� �߰�
        foreach (GameObject obj in _objectList)
        {
            string tName = obj.name;  // �� ������Ʈ�� �̸��� ������
            _prefabDic.Add(tName, obj);  // ������Ʈ �̸��� Ű�� ��ųʸ��� ���� (������ �̹����� ������ �غ�)
        }
    }

    private void OnEnable()
    {
        // �̹����� �����ǰų� ������Ʈ�� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯 ���
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        // ������Ʈ�� ��Ȱ��ȭ�Ǹ� �̺�Ʈ �ڵ鷯 ����
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    void Update()
    {
        /*
        if (Input.touchCount == 2)
        {
            // �� �հ��� ��ġ�� �����ɴϴ�.
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // ��ġ ���� �� �ʱ� �Ÿ��� ObjectPool�� �ʱ� ũ�� ����
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                previousTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                initialScale = ObjectPool.transform.localScale;
            }
            // ��ġ �̵� �� ObjectPool�� ������ ����
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                if (Mathf.Approximately(previousTouchDistance, 0)) return;

                // ���� �Ÿ��� �ʱ� �Ÿ��� ������ ���� ������ ���� ���
                float scaleFactor = currentTouchDistance / previousTouchDistance;
                ObjectPool.transform.localScale = initialScale * scaleFactor;  // ObjectPool�� ������ ����
            }
        }
        */

        // ȸ���� ������ ���� ���� �߰�


        // ������ ȸ������ �ʱ�ȭ���� �ʵ��� `initialTouchPosition`�� �� ���� �����ϵ��� ����
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // ��ġ�� ������ ������ �ʱ� ��ġ ��ġ�� ����
            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
            }

            // ��ġ �̵� �� ȸ�� ���� ����
            if (touch.phase == TouchPhase.Moved)
            {
                // ��ġ ��ġ�� ��ȭ�� �������� ȸ�� ���� ���
                float deltaX = touch.position.x - initialTouchPosition.x;
                float screenWidth = Screen.width;

                // ȭ���� ���̸� �������� �̵� ������ ȸ������ ��ȯ
                float rotationAmount = (deltaX / screenWidth) * 360f;

                // ���� ȸ�� ����
                accumulatedRotation += rotationAmount;

                // ���ŵ� ȸ���� ����
                ObjectPool.transform.rotation = Quaternion.Euler(0, accumulatedRotation, 0);

                // �ʱ� ��ġ ��ġ�� ���� ��ġ ��ġ�� ������Ʈ�Ͽ� ȸ���� ��� �����ǵ��� ����
                initialTouchPosition = touch.position;
            }
        }



    }


    // �̹����� ���� �νĵǾ��ų� ������Ʈ�� ��쿡 ȣ���
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // ���� �߰��� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);  // �̹����� �ش��ϴ� ������Ʈ�� ��ġ
        }

        // ������Ʈ�� �̹��� ó�� (���� �̹����� �����̰ų� ���°� ����� ���)
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);  // �̹����� �����ϴ� ������Ʈ�� ��ġ �� ȸ���� ������Ʈ
        }

        // ������ ������ �̹��� ó�� (�̹����� ������ ���� ��)
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            DisableImage(trackedImage);  // ������Ʈ ��Ȱ��ȭ
        }
    }

    // ������ �̹����� �����ϴ� ������Ʈ�� ��ġ�� ȸ���� ������Ʈ�ϰ�, ������Ʈ�� Ȱ��ȭ
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;  // ������ �̹����� �̸��� ������
        GameObject tObj = _prefabDic[name];  // �ش� �̸��� ���ε� ������Ʈ�� ��ųʸ����� ������
        tObj.transform.position = trackedImage.transform.position;  // �̹����� �ִ� ��ġ�� ������Ʈ ��ġ�� ����
        tObj.transform.rotation = trackedImage.transform.rotation * Quaternion.Euler(0, 180, 0);
        // �̹����� ȸ���� ���� ������Ʈ ȸ���� ����
        tObj.SetActive(true);  // ������Ʈ�� Ȱ��ȭ�Ͽ� ���� ǥ��

        // ������Ʈ�� ���� ������ UI�� ǥ��
        if (_objectDescriptions.ContainsKey(name))
        {
            string description = _objectDescriptions[name];
            objectDescriptionText.text = description;
            objectDescriptionText.gameObject.SetActive(true);
            tts.text = description; // ���� �ؽ�Ʈ�� TTS�� ����
        }
    }

    // ������ ������ �̹����� �����ϴ� ������Ʈ�� ��Ȱ��ȭ
    private void DisableImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;  // ������ �̹����� �̸��� ������
        if (_prefabDic.ContainsKey(name))
        {
            GameObject tObj = _prefabDic[name];  // �ش� �̸��� ���ε� ������Ʈ�� ��ųʸ����� ������
            tObj.SetActive(false);  // ������Ʈ�� ��Ȱ��ȭ�Ͽ� ������ ����
        }

        // ������Ʈ ���� UI �����
        objectDescriptionText.gameObject.SetActive(false);
    }
}
