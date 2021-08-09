using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//여기에 event 클래스 추가

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
        if (event_go != null)//인수에 게임 오브젝트가 비어있지 않으면
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
            type = this.getEventType(event_go); // 이벤트 타입을 가져온다
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:                 
                if(carried_item == Item.TYPE.IRON)//가지고있는 것이 철광석이라면
                {
                    ret = true;                   //이벤트 가능
                }
                if(carried_item == Item.TYPE.PLANT)//가지고있는것이 식물이라면
                {
                    ret = true;                     //이벤트 가능
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
                message = "수리한다";
                break;
        }
        return(message);
    }



}
