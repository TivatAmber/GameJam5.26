using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Order {
    None,
    LeftShort,
    LeftPressing,
    LeftLeave,
    LeftLong,
    RightShort,
    RightPressing,
    RightLeave,
}

public class OrderManager : Singleton<OrderManager>
{
    public Order nowOrder;
    public int pressingFrames;
    public bool canReceiveOrder;
    private bool pressing;
    private bool lstLeft;
    private bool left;
    private bool lstRight;
    private bool right;
    private float pressingTime;

    private void Start()
    {
        pressingTime = 0;
        nowOrder = Order.None;
    }
    private void Update()
    {
        if (!canReceiveOrder) return;
        if (pressing) {
            pressingTime += Time.deltaTime;
            pressingFrames += 1;
        }
        else {
            pressingTime = 0.0f;
            pressingFrames = 0;
        }

        lstLeft = left;
        lstRight = right;
        left = Input.GetMouseButton(0);
        right = Input.GetMouseButton(1);
        if (left || right)
        {
            if (left && pressingTime > 1.0f) nowOrder = Order.LeftLong;
            else if (left && pressing) nowOrder = Order.LeftPressing;
            else if (left && !pressing) nowOrder = Order.LeftShort;
            else if (right && pressing) nowOrder = Order.RightPressing;
            else if (right && !pressing) nowOrder = Order.RightShort;
            else nowOrder = Order.None;
            pressing = true;
        }
        else
        {
            if (lstLeft) nowOrder = Order.LeftLeave;
            else if (lstRight) nowOrder = Order.RightLeave;
            else nowOrder = Order.None;
            pressing = false;
        }
        // Debug.LogFormat("{0}, {1}, {2}, {3}, {4}", lstLeft, lstRight, left, right, nowOrder);
    }
}
