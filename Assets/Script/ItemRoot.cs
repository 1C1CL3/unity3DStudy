using System.Collections;
using System.Collections.Generic; // List�� ����ϱ� ���ؼ�
using UnityEngine;

public class Item
{
    public enum TYPE // ������ ����
    {
        NONE = -1,   // ����
        IRON = 0,    //ö����
        APPLE,       // ���
        PLANT,       // �Ĺ�
        NUM,         // =3
    }
}
public class ItemRoot : MonoBehaviour
{
    public GameObject ironPrefab;                         //������ iron
    public GameObject plantPrefab;                        //������ plant
    public GameObject applePrefab;                        //������ apple
    public List<Vector3> respawn_point;                   //���� ���� list

    public float step_timer = 0.0f;                       
    public static float RESPAWN_TIME_APPLE = 20.0f;       //��� ���� �ð� ���
    public static float RESPAWN_TIME_IRON = 12.0f;        //ö���� ���� �ð� ���
    public static float RESPAWN_TIME_PLANT = 6.0f;        //�Ĺ� ���� �ð� ���

    public float respawn_timer_apple = 0.0f;               //��� ���� �ð�
    public float respawn_timer_iron = 0.0f;                //ö���� ���� �ð�
    public float respawn_timer_plant = 0.0f;               //�Ĺ��� ���� �ð�

    // �������� ������ Item.type���� ��ȯ�ϴ� �޼���   
    public Item.TYPE getItemType(GameObject item_go)
    {

        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null)            //�μ��� ���� ���� ������Ʈ�� ������� ������ 
        {
            switch (item_go.tag)        //�±׷� �б�
            {
                case "Iron": type = Item.TYPE.IRON; break;
                case "Apple": type = Item.TYPE.APPLE; break;
                case "Plant": type = Item.TYPE.PLANT; break;
            }
        }
        return (type);
    }

    public void respawnIron()
    {
        // ö���� �������� �ν��Ͻ�ȭ
        GameObject go = GameObject.Instantiate(this.ironPrefab) as GameObject;
        // ö���� �������� ȹ��
        Vector3 pos = GameObject.Find("IronRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // ����� ��ġ�� �̵�
        go.transform.position = pos;

    }

    public void respawnApple()
    {
        // ö���� �������� �ν��Ͻ�ȭ
        GameObject go = GameObject.Instantiate(this.applePrefab) as GameObject;
        // ö���� �������� ȹ��
        Vector3 pos = GameObject.Find("AppleRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // ����� ��ġ�� �̵�
        go.transform.position = pos;
    }
    public void respawnPlant()// ����
    {
        //list �� ������� ������         
        if (this.respawn_point.Count>0) 
        {
            // �Ĺ� �������� �ν��Ͻ�ȭ
            GameObject go = GameObject.Instantiate(this.plantPrefab) as GameObject;        
            // �Ĺ� �������� ȹ��       
            Vector3 pos = GameObject.Find("PlantRespawn").transform.position;
            pos.y = 1.0f;       
            pos.x += Random.Range(-1.0f, 1.0f);        
            pos.z += Random.Range(-1.0f, 1.0f);
            // �Ĺ��� ��ġ�� �̵�
            go.transform.position = pos;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        //�޸� ���� Ȯ��
        this.respawn_point = new List<Vector3>();
        // "PlantRespawn"�±װ� ���� ��� ������Ʈ�� �迭�� ����
        GameObject[] respawn = GameObject.FindGameObjectsWithTag("PlantRespawn");
        
        //�迭 respawns ���� �� GameObject�� ������� ó�� 
        foreach(GameObject go in respawn)
        {
            //������ ȹ��
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if(renderer != null)//�������� �����ϸ�
            {
                renderer.enabled = false;// �� �������� ������ �ʰ�
            }
            //�������� list�� ��ġ ������ �߰�
            this.respawn_point.Add(go.transform.position);
        }
        //����� ���� ������ ����ϰ� �������� ������ �ʰ�
        GameObject applerespwn = GameObject.Find("AppleRespawn");
        applerespwn.GetComponent<MeshRenderer>().enabled = false;
        //ö������ ���� ������ ����ϰ� �������� ������ �ʰ�
        GameObject ironrespwn = GameObject.Find("IronRespawn");
        ironrespwn.GetComponent<MeshRenderer>().enabled = false;

        this.respawnIron();//ö������ �ϳ� ����
        this.respawnPlant();// �Ĺ��� �ϳ� ����
    }

    // Update is called once per frame
    void Update()
    {
        respawn_timer_iron += Time.deltaTime;
        respawn_timer_apple += Time.deltaTime;
        respawn_timer_plant += Time.deltaTime;

        if(respawn_timer_apple > RESPAWN_TIME_APPLE)
        {
            respawn_timer_apple = 0.0f;
            this.respawnApple();
        }
        if (respawn_timer_iron > RESPAWN_TIME_IRON)
        {
            respawn_timer_iron = 0.0f;
            this.respawnIron();
        }
        if (respawn_timer_plant > RESPAWN_TIME_PLANT)
        {
            respawn_timer_plant = 0.0f;
            this.respawnPlant();
        }

    }
}
