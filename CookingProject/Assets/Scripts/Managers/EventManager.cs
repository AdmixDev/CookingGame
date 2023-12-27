using System.Collections.Generic;

public static class EventManager
{
    public delegate void EventReceiver(params object[] parameters);

    static Dictionary<EventType, EventReceiver> _events = new Dictionary<EventType, EventReceiver>();

    public static void Subscribe(EventType eventType, EventReceiver listener)
    {
        if (!_events.ContainsKey(eventType))
            _events.Add(eventType, listener);
        else
            _events[eventType] += listener;
    }

    public static void UnSubscribe(EventType eventType, EventReceiver listener)
    {
        if (_events.ContainsKey(eventType))
        {
            _events[eventType] -= listener;

            if (_events[eventType] == null)
                _events.Remove(eventType);
        }
    }

    public static void Trigger(EventType eventType, params object[] parameters)
    {
        if (_events.ContainsKey(eventType))
            _events[eventType](parameters);
    }

    public static void Clear()
    {
        _events.Clear();
    }

    public enum EventType
    {
        NewGame,
        Win,
        GameOver,
        ResetGame
    }
}