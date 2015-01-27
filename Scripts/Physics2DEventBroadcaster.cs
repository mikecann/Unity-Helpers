using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace UnityHelpers
{
    public class Physics2DEventBroadcaster : MonoBehaviour
    {
        public Phyics2DTriggerEvent onTriggerEnter = new Phyics2DTriggerEvent();
        public Phyics2DTriggerEvent onTriggerExit = new Phyics2DTriggerEvent();
        public Phyics2DTriggerEvent onTriggerStay = new Phyics2DTriggerEvent();
        public Phyics2DCollisionEvent onCollisionEnter = new Phyics2DCollisionEvent();
        public Phyics2DCollisionEvent onCollisionExit = new Phyics2DCollisionEvent();
        public Phyics2DCollisionEvent onCollisionStay = new Phyics2DCollisionEvent();

        public void OnTriggerEnter2D(Collider2D other)
        {
            onTriggerEnter.Invoke(other);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            onTriggerExit.Invoke(other);
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            onTriggerStay.Invoke(other);
        }

        public void OnCollisionEnter2D(Collision2D coll)
        {
            onCollisionEnter.Invoke(coll);
        }

        public void OnCollisionExit2D(Collision2D coll)
        {
            onCollisionExit.Invoke(coll);
        }

        public void OnCollisionStay2D(Collision2D coll)
        {
            onCollisionStay.Invoke(coll);
        }       
    }

    public class Phyics2DTriggerEvent : UnityEvent<Collider2D> { }
    public class Phyics2DCollisionEvent : UnityEvent<Collision2D> { }
}
