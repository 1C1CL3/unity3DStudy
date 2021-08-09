using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float MoveAreaRadius = 15.0f;
    public static float MoveSpeed = 5.0f;

    private GameObject closestItem = null; // 플레이어의 정면에 있는 게임 오브젝트
    private GameObject carriedItem = null; // 플레이어가 들어 올린 게임 오브젝트
    private ItemRoot item_root = null;     // ItemRoot 스크립트를 가짐.
    public GUIStyle guistyle;              // 폰트 스타일.

    private GameObject close_event = null;  //주목하고 있는 이벤트를 저장
    private EventRoot event_root = null;    //eventRoot클래스를 사용하기위한 변수
    private GameObject rocket_model = null; // 우주선의 모델을 사용하기 위한 변수

    private struct Key
    {
        public bool up;
        public bool down;
        public bool right;
        public bool left;
        public bool action;
        public bool pick;
    }
    private Key key;

    public enum STEP
    {
        NONE = -1,
        MOVE = 0,
        REPAIRING,
        EATING,
        NUM
    }

    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    void OnGUI()
    {
        float x = 20.0f;
        float y = Screen.height - 40.0f;

        if(this.carriedItem != null)
        {
            GUI.Label(new Rect(x, y, 200.0f, 20.0f),"Z:버린다",guistyle);
            GUI.Label(new Rect(x + 100.0f, y, 200.0f, 20.0f), "X:먹는다", guistyle);
        }
        else
        {
            if (this.closestItem != null)
            {
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:줍는다", guistyle);
            }
        }

        switch(this.step)
        {
            case STEP.EATING:
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "우걱우걱우물우물...", guistyle);
                break;
        }

    }


    private void get_input()
    {

        this.key.up = false;
        this.key.down = false;
        this.key.right = false;
        this.key.left = false;

        this.key.pick = Input.GetKeyDown(KeyCode.Z);

        // ↑키가 눌릴때 true 입력
        this.key.up |= Input.GetKey(KeyCode.UpArrow);
        this.key.up |= Input.GetKey(KeyCode.W);

        // ↓키가 눌릴때 true 입력
        this.key.down |= Input.GetKey(KeyCode.DownArrow);
        this.key.down |= Input.GetKey(KeyCode.S);

        // →키가 눌릴때 true 입력
        this.key.right |= Input.GetKey(KeyCode.RightArrow);
        this.key.right |= Input.GetKey(KeyCode.D);

        // ←키가 눌릴때 true 입력
        this.key.left |= Input.GetKey(KeyCode.LeftArrow);
        this.key.left |= Input.GetKey(KeyCode.A);

        //z키가 눌렸으면 true
        this.key.pick |= Input.GetKeyDown(KeyCode.Z);
        this.key.pick |= Input.GetKeyDown(KeyCode.Mouse0);
        

        //x키가 눌렸으면 true
        this.key.action |= Input.GetKeyDown(KeyCode.X);
        this.key.action |= Input.GetKeyDown(KeyCode.Mouse1);

        
    }

    private void moveControl()
    {
        bool isMoved = false;
        
        Vector3 moveVector = Vector3.zero; // 이동용 벡터
        Vector3 postion = this.transform.position; // 현재 위치 보관


        if (this.key.right)
        {
            moveVector += Vector3.right;
            isMoved = true;
        }
        if (this.key.left)
        {
            moveVector += Vector3.left;
            isMoved = true;
        }
        if (this.key.up)
        {
            moveVector += Vector3.forward;
            isMoved = true;
        }
        if (this.key.down)
        {
            moveVector += Vector3.back;
            isMoved = true;
        }

        moveVector.Normalize();  //길이를 1로
        moveVector *= MoveSpeed * Time.deltaTime; //거= 속*시
        postion += moveVector;   //위치를 이동
        postion.y = 0.0f;        // 높이를 0으로 합니다
       
        //새로 구한 위치의 높이를 현재 높이로 되돌린다.
        if (postion.magnitude > MoveAreaRadius)
        {
            postion.Normalize();
            postion *= MoveAreaRadius;
        }

        //실제 위치를 새로 구한 위치로 변경한다.
        this.transform.position = postion;

        //이동 벡터의 길이가 0.01보다 큰경우       
        //어느 정도 이상의 이동한 경우.
        if (moveVector.magnitude > 0.01f)
        {           
            //캐릭터의 방향을 천천히 바꾼다.
            Quaternion q = Quaternion.LookRotation(moveVector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.1f);
            Debug.Log("transform.rotation작동중");
        }
    }

    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)                        //줍기 버리기 키가 눌리지 않았으면.
            {
                break;                                 //아무것도 하지 않고 메서드 종료
            }
            if (this.carriedItem == null)              // 들고 있는 아이템이 없고.
            {
                if (this.closestItem == null)          // 주목중인 아이템이 없으면
                {
                    break;                             // 아무것도 하지 않고 메서드 종료
                }
                //주목 중인 아이템을 들어 올린다
                this.carriedItem = this.closestItem;
                //들고 있는 아이템을 자신의 자식으로 설정.
                this.carriedItem.transform.parent = this.transform;
                // 2.0f 위에 배치 (머리 위로 이동)
                this.carriedItem.transform.localPosition = Vector3.up * 2.0f;
                // 주목 중인 아이템을 없엔다.
                this.closestItem = null;
            }
            else// 들고있는 아이템이 있을경우
            {
                //들고있는 아이템을 약(1.0f)앞으로 이동시켜서
                this.carriedItem.transform.localPosition = Vector3.forward * 1.0f;
                this.carriedItem.transform.parent = null;                   // 자식 설정을 해제
                this.carriedItem = null;                                    // 들고 잇는 아이템을 없엔다.
            }
        } while (false);
    }

    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = this.transform.TransformDirection(Vector3.forward); // 자신이 현재 향하고 있는 방향을 보관.
            Vector3 to_other = other.transform.position - this.transform.position;// 자신쪽에서 본 아이템의 방향을 보관.
            heading.y = 0.0f;
            to_other.y = 0.0f;

            heading.Normalize();                        //백터의 길이를 1로 하고 방향만가져온다
            to_other.Normalize();                       //백터의 길이를 1로 하고 방향만가져온다
            float dp = Vector3.Dot(heading, to_other);  // 양쪽의 벡터의 내적을 취득 바라보고있는 방향 기준으로 내적1이며 맨뒤는 -1이다. 
            if (dp < Mathf.Cos(45.0f))                  // 내적이 cos45 값 미만이면 cos45 = 2/루트2 = 0.707...
            {
                break;                                  // 루푸를 빠져 나간다
            }
            ret = true;                                 // 내적이 45도의 코사인 값 이상이면 정면에 있다.
        } while (false);
        return (ret);
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;

        //트리거의 게임 오브젝트의 레이어 설정이 Item이라면,
        if(other_go.layer == LayerMask.NameToLayer("Item"))
        {
            //아무것도 주목하고 있지 않다면
            if(this.closestItem == null)
            {
                if (this.is_other_in_view(other_go))
                {
                    this.closestItem = other_go;
                }
                else if (this.closestItem == other_go)
                {
                    if(!this.is_other_in_view(other_go))
                    {
                        this.closestItem = null;
                    }
                }
            }
        }

        else if(other_go.layer == LayerMask.NameToLayer("Event"))
        {
            // 아무것도 주목하고있지 않다면

            if(this.close_event == null)
            {
                if(this.is_other_in_view(other_go))//정면에 있으면
                {
                    this.close_event = other_go;    // 주목한다
                }
            }
            //무엇인가 주목하고있으면
            else if(this.close_event == other_go)
            {
                if (!this.is_other_in_view(other_go))//정면에 없으면
                {
                    this.close_event = null;    //주목을 그만둔다
                }
            }
        }
    }
    private bool is_event_ignitable()
    {
        bool ret = false;

        do
        {
            if (this.close_event == null)           //주목 이벤트가 없으면
            {
                break;                              // false 반환  주목하고있다는게 없으면 그대로 끝내버리는거고
            }
            //carried_item_type들고있는 아이템 종류를 가져온다
            Item.TYPE carried_item_type = this.item_root.getItemType(this.carriedItem);

            //들고 있는아이템 종류와 주목하는 이벤트 종류에서
            //이벤트가 가능한지 판정하고 이벤트 불가라면 false를 반환한다
            if (!this.event_root.isEventIgnitable(carried_item_type, this.close_event))
            {
                break;
            }
            ret = true;// 여기까지 오면 이벤트를 시작할 수 있다고 판정
        } while (false);
        return (ret);
    }



    private void OnTriggerExit(Collider other)
    {
        if(this.closestItem == other.gameObject)
        {
            this.closestItem = null;                   //주목을 그만둔다.
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        this.step = STEP.NONE;      //현 단계 상태를 초기화
        this.next_step = STEP.MOVE; // 다음단계를 초기화
        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.guistyle.fontSize = 16;

        this.event_root = GameObject.Find("GameRoot").GetComponent<EventRoot>();
        this.rocket_model = GameObject.Find("rocket").transform.Find("rocket_model").gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        this.get_input(); // 입력 함수 업데이트
        this.step_timer += Time.deltaTime;
        float eat_time= 2.0f;                       // 사과는 2초에 걸쳐 먹는다

        // 상태가 변했을때------------------
        if(this.next_step == STEP.NONE)             //다음 예정이 없으면
        {
            switch(this.step)                   
            {
                case STEP.MOVE:                     //이동중 상태의 처리
                    do
                    {
                        if (!this.key.action)       //액션 키 눌려있지 않다
                        {
                            break;                  //루프 탈출
                        }
                        if (this.carriedItem != null)// 가지고 있는 아이템 판별
                        {
                            Item.TYPE carriedItemType = this.item_root.getItemType(this.carriedItem);

                            switch (carriedItemType)
                            {
                                case Item.TYPE.APPLE:           //사과라면
                                case Item.TYPE.PLANT:           //식물이라면
                                    this.next_step = STEP.EATING;// 식사중 상태로 이행
                                    break;
                            }
                        }
                    } while (false);
                    break;

                case STEP.EATING:                       //식사중 상태의 처리
                    if(this.step_timer > eat_time)      //2초대기
                    {
                        this.next_step = STEP.MOVE;     //이동 상태로 이행
                    }
                    break;
                    
            }
        }
        // 상태가 변화하지 않을때
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            switch (this.step)
            {
                case STEP.MOVE:
                    break;

                case STEP.EATING:               // 식사중의 처리
                    if(this.carriedItem !=null) // 가지고 있던 아이템을 폐기
                    {
                        GameObject.Destroy(this.carriedItem);
                        this.carriedItem = null;
                    }
                    break;

            }
            this.step_timer = 0.0f;
        }

        // 각 상황에서 반복---------------
        switch (this.step)
        {
            case STEP.MOVE:
                this.moveControl();
                this.pick_or_drop_control();
                break;
        }
       
    }
}
