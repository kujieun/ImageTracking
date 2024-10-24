using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ARTrackedImg : MonoBehaviour
{

    // ARTrackedImageManager�� ���� AR �̹��� ���� ����
    public ARTrackedImageManager trackedImageManager;

    // 3D ������Ʈ ����Ʈ
    public List<GameObject> _objectList = new List<GameObject>();

    // �̹��� �̸��� Ű��, �ش� �̹����� ������ �� ǥ���� 3D ������Ʈ�� ������ ������ ��ųʸ�
    private Dictionary<string, GameObject> _prefabDic = new Dictionary<string, GameObject>();

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
        tObj.transform.rotation = trackedImage.transform.rotation;  // �̹����� ȸ���� ���� ������Ʈ ȸ���� ����
        tObj.SetActive(true);  // ������Ʈ�� Ȱ��ȭ�Ͽ� ���� ǥ��
    }

    // ������ ������ �̹����� �����ϴ� ������Ʈ�� ��Ȱ��ȭ
    private void DisableImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;  // ������ �̹����� �̸��� ������
        if (_prefabDic.ContainsKey(name))
        {
            GameObject tObj = _prefabDic[name];  // �ش� �̸��� ���ε� ������Ʈ�� ��ųʸ����� ������
            tObj.SetActive(false);  // ������Ʈ�� ��Ȱ��ȭ
        }
    }
}
