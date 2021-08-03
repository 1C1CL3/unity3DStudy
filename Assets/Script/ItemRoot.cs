using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해서
using UnityEngine;

public class Item
{
    public enum TYPE // 아이템 종류
    {
        NONE = -1,   // 없음
        IRON = 0,    //철광석
        APPLE,       // 사과
        PLANT,       // 식물
        NUM,         // =3
    }
}
public class ItemRoot : MonoBehaviour
{
    public GameObject ironPrefab;                         //프리팹 iron
    public GameObject plantPrefab;                        //프리팹 plant
    public GameObject applePrefab;                        //프리팹 apple
    public List<Vector3> respawn_point;                   //출현 지접 list

    public float step_timer = 0.0f;                       
    public static float RESPAWN_TIME_APPLE = 20.0f;       //사과 출현 시간 상수
    public static float RESPAWN_TIME_IRON = 12.0f;        //철광석 출현 시간 상수
    public static float RESPAWN_TIME_PLANT = 6.0f;        //식물 출현 시간 상수

    public float respawn_timer_apple = 0.0f;               //사과 출현 시간
    public float respawn_timer_iron = 0.0f;                //철광석 출현 시간
    public float respawn_timer_plant = 0.0f;               //식물의 출현 시간

    // 아이템의 종류를 Item.type으로 변환하는 메서드   
    public Item.TYPE getItemType(GameObject item_go)
    {

        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null)            //인수로 받은 게임 오브젝트가 비어있지 않으면 
        {
            switch (item_go.tag)        //태그로 분기
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
        // 철광석 프리팹을 인스턴스화
        GameObject go = GameObject.Instantiate(this.ironPrefab) as GameObject;
        // 철광석 출현지점 획득
        Vector3 pos = GameObject.Find("IronRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // 사과의 위치를 이동
        go.transform.position = pos;

    }

    public void respawnApple()
    {
        // 철광석 프리팹을 인스턴스화
        GameObject go = GameObject.Instantiate(this.applePrefab) as GameObject;
        // 철광석 출현지점 획득
        Vector3 pos = GameObject.Find("AppleRespawn").transform.position;
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // 사과의 위치를 이동
        go.transform.position = pos;
    }
    public void respawnPlant()// 수정
    {
        //list 가 비어있지 않으면         
        if (this.respawn_point.Count>0) 
        {
            // 식물 프리팹을 인스턴스화
            GameObject go = GameObject.Instantiate(this.plantPrefab) as GameObject;        
            // 식물 출현지점 획득       
            Vector3 pos = GameObject.Find("PlantRespawn").transform.position;
            pos.y = 1.0f;       
            pos.x += Random.Range(-1.0f, 1.0f);        
            pos.z += Random.Range(-1.0f, 1.0f);
            // 식물의 위치를 이동
            go.transform.position = pos;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        //메모리 영역 확보
        this.respawn_point = new List<Vector3>();
        // "PlantRespawn"태그가 붙은 모든 오브젝트를 배열에 저장
        GameObject[] respawn = GameObject.FindGameObjectsWithTag("PlantRespawn");
        
        //배열 respawns 내의 각 GameObject를 순서대로 처리 
        foreach(GameObject go in respawn)
        {
            //렌더러 획득
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if(renderer != null)//랜더러가 존재하면
            {
                renderer.enabled = false;// 그 랜더러를 보이지 않게
            }
            //출현지점 list에 위치 정보를 추가
            this.respawn_point.Add(go.transform.position);
        }
        //사과의 출현 지점을 취득하고 렌더러를 보이지 않게
        GameObject applerespwn = GameObject.Find("AppleRespawn");
        applerespwn.GetComponent<MeshRenderer>().enabled = false;
        //철광석의 출현 지점을 취득하고 랜더러를 보이지 않게
        GameObject ironrespwn = GameObject.Find("IronRespawn");
        ironrespwn.GetComponent<MeshRenderer>().enabled = false;

        this.respawnIron();//철광석을 하나 생성
        this.respawnPlant();// 식물을 하나 생성
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
