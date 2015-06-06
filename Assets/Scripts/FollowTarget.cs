using System;
using UnityEngine;




    public class FollowTarget : MonoBehaviour
    {
        public Transform Target;
        public bool Autocenter = false;
        public Vector3 InitialOffset = new Vector3(0f, 18f, -10f);
        private Vector3 additionalOffset = new Vector3(0,0,0);

        public float HorizontalBorder = 0.2f;
        public float VerticalBorder = 0.2f;
        public float MoveSpeed = 9f;
        public float MaxHorizontalOffset = 5f;
        public float MaxVerticalOffset = 5f;
        public float MaxHeightOffset = 5f;

        float horBorderLeft;
        float horBorderRight;
        float vertBorderTop;
        float vertBorderBottom;


        float raw = 0;
        float yOffset = 0;

        void Awake()
        {
            horBorderLeft = Screen.width * HorizontalBorder;
            horBorderRight = Screen.width - HorizontalBorder * Screen.width;

            vertBorderBottom = Screen.height * VerticalBorder;
            vertBorderTop = Screen.height - Screen.height * VerticalBorder;

        }

        private void Update()
        {
            float delta = 0;
            float horSpd = 0;
            float vertSpd = 0;
            

            if (Target != null)
            {

                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    raw += Input.GetAxis("Mouse ScrollWheel");
                    raw = Mathf.Clamp(raw, -0.5f, 0.7f);
                    yOffset = MaxHeightOffset * (-raw / 0.7f); 

                }



                if (Input.mousePosition.x > horBorderRight)
                {
                    delta = Input.mousePosition.x - horBorderRight;
                    delta = Mathf.Clamp(delta, 0, horBorderLeft);
                    horSpd = MoveSpeed * (delta / horBorderLeft);
                   // Debug.Log(horSpd);

                }
                else if (Input.mousePosition.x < horBorderLeft)
                {
                    delta = Input.mousePosition.x - horBorderLeft;
                    delta = Mathf.Clamp(delta, -horBorderLeft, 0);
                    horSpd = MoveSpeed * (delta / horBorderLeft);
                }
                else if (Autocenter == true)
                {
                    if (additionalOffset.x > 0.2f)
                    {
                        additionalOffset.x -= MoveSpeed * 0.5f * Time.deltaTime;
                    }
                    else if (additionalOffset.x < -0.2f)
                    {
                        additionalOffset.x += MoveSpeed * 0.5f * Time.deltaTime;
                    }
                }


                if (Input.mousePosition.y > vertBorderTop)
                {
                    delta = Input.mousePosition.y - vertBorderTop;
                    delta = Mathf.Clamp(delta, 0, vertBorderBottom);
                    vertSpd = MoveSpeed * (delta / vertBorderBottom);
                }
                else if (Input.mousePosition.y < vertBorderBottom)
                {
                    delta = Input.mousePosition.y - vertBorderBottom;
                    delta = Mathf.Clamp(delta, -vertBorderBottom, 0);
                    vertSpd = MoveSpeed * (delta / vertBorderBottom);
                }



                additionalOffset.x += horSpd * Time.deltaTime;
                additionalOffset.x = Mathf.Clamp(additionalOffset.x, -MaxHorizontalOffset, MaxHorizontalOffset);

                additionalOffset.z += vertSpd * Time.deltaTime;
                additionalOffset.z = Mathf.Clamp(additionalOffset.z, -MaxVerticalOffset, MaxVerticalOffset);

                additionalOffset.y = yOffset;

               // Debug.Log("X:" + Input.mousePosition.x + "Y:" + Input.mousePosition.y);
                transform.position = Target.position + InitialOffset + additionalOffset;
            }
            
        }
    }

