using System;
using UnityEngine;




    public class FollowTarget : MonoBehaviour
    {
        public Transform Target;
        public Vector3 Offset = new Vector3(0f, 7.5f, 0f);


        private void Update()
        {
            if (Target != null)
            {
                transform.position = Target.position + Offset;
            }
            
        }
    }

