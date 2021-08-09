using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//���⿡ event Ŭ���� �߰�

public class Event
{
    public enum TYPE
    {
        NONE = -1,
        ROCKET = 0,
        NUM,
    };
};


public class EventRoot : MonoBehaviour
{
    public Event.TYPE getEventType(GameObject event_go)
    {
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)//�μ��� ���� ������Ʈ�� ������� ������
        {
            if(event_go.tag =="Rocket")
            {
                type = Event.TYPE.ROCKET;
            }
        }
        return (type);
    }
    public bool isEventIgnitable(Item.TYPE carried_item, GameObject event_go)
    {
        bool ret = false;
        Event.TYPE type = Event.TYPE.NONE;

        if (event_go != null)
        {
            type = this.getEventType(event_go); // �̺�Ʈ Ÿ���� �����´�
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:                 
                if(carried_item == Item.TYPE.IRON)//�������ִ� ���� ö�����̶��
                {
                    ret = true;                   //�̺�Ʈ ����
                }
                if(carried_item == Item.TYPE.PLANT)//�������ִ°��� �Ĺ��̶��
                {
                    ret = true;                     //�̺�Ʈ ����
                }
                break;
        }
        return (ret);
    }
    public string getIgnitableMessage(GameObject event_go)
    {
        string message="";
        Event.TYPE type = Event.TYPE.NONE;
        if(event_go != null)
        {
            type = this.getEventType(event_go);
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:
                message = "�����Ѵ�";
                break;
        }
        return(message);
    }



}
