using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float MoveAreaRadius = 15.0f;
    public static float MoveSpeed = 5.0f;

    private GameObject closestItem = null; // �÷��̾��� ���鿡 �ִ� ���� ������Ʈ
    private GameObject carriedItem = null; // �÷��̾ ��� �ø� ���� ������Ʈ
    private ItemRoot item_root = null;     // ItemRoot ��ũ��Ʈ�� ����.
    public GUIStyle guistyle;              // ��Ʈ ��Ÿ��.

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
            GUI.Label(new Rect(x, y, 200.0f, 20.0f),"Z:������",guistyle);
            GUI.Label(new Rect(x + 100.0f, y, 200.0f, 20.0f), "X:�Դ´�", guistyle);
        }
        else
        {
            if (this.closestItem != null)
            {
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:�ݴ´�", guistyle);
            }
        }

        switch(this.step)
        {
            case STEP.EATING:
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "��ƿ�ƿ칰�칰...", guistyle);
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

        // ��Ű�� ������ true �Է�
        this.key.up |= Input.GetKey(KeyCode.UpArrow);
        this.key.up |= Input.GetKey(KeyCode.W);

        // ��Ű�� ������ true �Է�
        this.key.down |= Input.GetKey(KeyCode.DownArrow);
        this.key.down |= Input.GetKey(KeyCode.S);

        // ��Ű�� ������ true �Է�
        this.key.right |= Input.GetKey(KeyCode.RightArrow);
        this.key.right |= Input.GetKey(KeyCode.D);

        // ��Ű�� ������ true �Է�
        this.key.left |= Input.GetKey(KeyCode.LeftArrow);
        this.key.left |= Input.GetKey(KeyCode.A);

        //zŰ�� �������� true
        this.key.pick |= Input.GetKeyDown(KeyCode.Z);
        this.key.pick |= Input.GetKeyDown(KeyCode.Mouse0);
        

        //xŰ�� �������� true
        this.key.action |= Input.GetKeyDown(KeyCode.X);
        this.key.action |= Input.GetKeyDown(KeyCode.Mouse1);

        
    }

    private void moveControl()
    {
        bool isMoved = false;
        Vector3 moveVector = Vector3.zero; // �̵��� ����
        Vector3 postion = this.transform.position; // ���� ��ġ ����


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

        moveVector.Normalize();  //���̸� 1��
        moveVector *= MoveSpeed * Time.deltaTime; //��= ��*��
        postion += moveVector;   //��ġ�� �̵�
        postion.y = 0.0f;        // ���̸� 0���� �մϴ�
       
        //���� ���� ��ġ�� ���̸� ���� ���̷� �ǵ�����.
        if (postion.magnitude > MoveAreaRadius)
        {
            postion.Normalize();
            postion *= MoveAreaRadius;
        }

        //���� ��ġ�� ���� ���� ��ġ�� �����Ѵ�.
        this.transform.position = postion;

        //�̵� ������ ���̰� 0.01���� ū���       
        //��� ���� �̻��� �̵��� ���.
        if (moveVector.magnitude > 0.01f)
        {
            //ĳ������ ������ õõ�� �ٲ۴�.
            Quaternion q = Quaternion.LookRotation(moveVector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.1f);
        }
    }

    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)                        //�ݱ� ������ Ű�� ������ �ʾ�����.
            {
                break;                                 //�ƹ��͵� ���� �ʰ� �޼��� ����
            }
            if (this.carriedItem == null)              // ��� �ִ� �������� ����.
            {
                if (this.closestItem == null)          // �ָ����� �������� ������
                {
                    break;                             // �ƹ��͵� ���� �ʰ� �޼��� ����
                }
                //�ָ� ���� �������� ��� �ø���
                this.carriedItem = this.closestItem;
                //��� �ִ� �������� �ڽ��� �ڽ����� ����.
                this.carriedItem.transform.parent = this.transform;
                // 2.0f ���� ��ġ (�Ӹ� ���� �̵�)
                this.carriedItem.transform.localPosition = Vector3.up * 2.0f;
                // �ָ� ���� �������� ������.
                this.closestItem = null;
            }
            else// ����ִ� �������� �������
            {
                //����ִ� �������� ��(1.0f)������ �̵����Ѽ�
                this.carriedItem.transform.localPosition = Vector3.forward * 1.0f;
                this.carriedItem.transform.parent = null;                   // �ڽ� ������ ����
                this.carriedItem = null;                                    // ��� �մ� �������� ������.
            }
        } while (false);
    }

    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = this.transform.TransformDirection(Vector3.forward); // �ڽ��� ���� ���ϰ� �ִ� ������ ����.
            Vector3 to_other = other.transform.position - this.transform.position;// �ڽ��ʿ��� �� �������� ������ ����.
            heading.y = 0.0f;
            to_other.y = 0.0f;

            heading.Normalize();//������ ���̸� 1�� �ϰ� ���⸸�����´�
            to_other.Normalize();//������ ���̸� 1�� �ϰ� ���⸸�����´�
            float dp = Vector3.Dot(heading, to_other);// ������ ������ ������ ��� �ٶ󺸰��ִ� ���� �������� ����1�̸� �ǵڴ� -1�̴�. 
            if (dp < Mathf.Cos(45.0f))// ������ cos45 �� �̸��̸� cos45 = 2/��Ʈ2 = 0.707...
            {
                break;// ��Ǫ�� ���� ������
            }
            ret = true; // ������ 45���� �ڻ��� �� �̻��̸� ���鿡 �ִ�.
        } while (false);
        return (ret);
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;

        //Ʈ������ ���� ������Ʈ�� ���̾� ������ Item�̶��,
        if(other_go.layer == LayerMask.NameToLayer("Item"))
        {
            //�ƹ��͵� �ָ��ϰ� ���� �ʴٸ�
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
    }

    private void OnTriggerExit(Collider other)
    {
        if(this.closestItem == other.gameObject)
        {
            this.closestItem = null;                   //�ָ��� �׸��д�.
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        this.step = STEP.NONE;
        this.next_step = STEP.MOVE;
        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.guistyle.fontSize = 16;
    }
   

    // Update is called once per frame
    void Update()
    {
        this.get_input(); // �Է� �Լ� ������Ʈ
        this.step_timer += Time.deltaTime;
        float eat_time= 2.0f;                   // ����� 2�ʿ� ���� �Դ´�

        // ���°� ��������------------------
        if(this.next_step == STEP.NONE)         //���� ������ ������
        {
            switch(this.step)                   
            {
                case STEP.MOVE:                 //�̵��� ������ ó��
                    do
                    {
                        if (!this.key.action)   //�׼� Ű �������� �ʴ�
                        {
                            break;              //���� Ż��
                        }
                        if (this.carriedItem != null)// ������ �ִ� ������ �Ǻ�
                        {
                            Item.TYPE carriedItemType = this.item_root.getItemType(this.carriedItem);

                            switch (carriedItemType)
                            {
                                case Item.TYPE.APPLE: //������
                                case Item.TYPE.PLANT: //�Ĺ��̶��
                                    this.next_step = STEP.EATING;// �Ļ��� ���·� ����
                                    break;
                            }
                        }
                    } while (false);
                    break;

                case STEP.EATING:                       //�Ļ��� ������ ó��
                    if(this.step_timer > eat_time)      //2�ʴ��
                    {
                        this.next_step = STEP.MOVE;     //�̵� ���·� ����
                    }
                    break;
                    
            }
        }
        // ���°� ��ȭ���� ������
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            switch (this.step)
            {
                case STEP.MOVE:
                    break;

                case STEP.EATING:               // �Ļ����� ó��
                    if(this.carriedItem !=null) // ������ �ִ� �������� ���
                    {
                        GameObject.Destroy(this.carriedItem);
                        this.carriedItem = null;
                    }
                    break;

            }
            this.step_timer = 0.0f;
        }

        // �� ��Ȳ���� �ݺ�---------------
        switch (this.step)
        {
            case STEP.MOVE:
                this.moveControl();
                this.pick_or_drop_control();
                break;
        }

       
    }
}
