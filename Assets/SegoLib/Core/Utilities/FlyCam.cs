using UnityEngine;
using System.Collections;

namespace SegoLib
{
    public class FlyCam : MonoBehaviour
    {

        /*
        Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
        Converted to C# 27-02-13 - no credit wanted.
        Simple flycam I made, since I couldn't find any others made public.  
        Made simple to use (drag and drop, done) for regular keyboard layout  
        wasd : basic movement
        shift : Makes camera accelerate
        space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/


        [SerializeField] private float m_mainSpeed = 100.0f; //regular speed
        [SerializeField] private float m_shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
        [SerializeField] private float m_maxShift = 1000.0f; //Maximum speed when holding shift
        [SerializeField] private float m_sensitivity = 0.25f; //How sensitive it with mouse
        private Vector3 m_lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
        private float m_totalRun = 1.0f;

        void Update()
        {
            m_lastMouse = Input.mousePosition - m_lastMouse;
            m_lastMouse = new Vector3(-m_lastMouse.y * m_sensitivity, m_lastMouse.x * m_sensitivity, 0);
            m_lastMouse = new Vector3(transform.eulerAngles.x + m_lastMouse.x, transform.eulerAngles.y + m_lastMouse.y, 0);
            transform.eulerAngles = m_lastMouse;
            m_lastMouse = Input.mousePosition;
            //Mouse  camera angle done.  

            //Keyboard commands
            Vector3 p = GetBaseInput();
            if (p.sqrMagnitude > 0) { // only move while a direction key is pressed
                if (Input.GetKey(KeyCode.LeftShift)) {
                    m_totalRun += Time.deltaTime;
                    p = p * m_totalRun * m_shiftAdd;
                    p.x = Mathf.Clamp(p.x, -m_maxShift, m_maxShift);
                    p.y = Mathf.Clamp(p.y, -m_maxShift, m_maxShift);
                    p.z = Mathf.Clamp(p.z, -m_maxShift, m_maxShift);
                } else {
                    m_totalRun = Mathf.Clamp(m_totalRun * 0.5f, 1f, 1000f);
                    p = p * m_mainSpeed;
                }

                p = p * Time.deltaTime;
                Vector3 newPosition = transform.position;
                if (Input.GetKey(KeyCode.Space)) { //If player wants to move on X and Z axis only
                    transform.Translate(p);
                    newPosition.x = transform.position.x;
                    newPosition.z = transform.position.z;
                    transform.position = newPosition;
                } else {
                    transform.Translate(p);
                }
            }
        }

        private Vector3 GetBaseInput()
        { //returns the basic values, if it's 0 than it's not active.
            Vector3 p_Velocity = new Vector3();
            if (Input.GetKey(KeyCode.W)) {
                p_Velocity += new Vector3(0, 0, 1);
            }
            if (Input.GetKey(KeyCode.S)) {
                p_Velocity += new Vector3(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.A)) {
                p_Velocity += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D)) {
                p_Velocity += new Vector3(1, 0, 0);
            }
            return p_Velocity;
        }
    } 
}