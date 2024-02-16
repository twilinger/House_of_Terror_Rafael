using System;
using UnityEngine;

namespace Common.Scripts
{
    [RequireComponent(typeof(Activator))]
    public class movement : MonoBehaviour
    {

        #region Inspector
        
        [SerializeField] private float speed = 100f;
        [SerializeField] private float sprintSpeedBonus = 50f;
        [SerializeField] private int GravityMultiplier = 1;

        [Header("Relations")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody physicsBody = null;
        private float _lastHorisontalInput=0;
        private float _lastVerticalInput=0;
        

        #endregion


        #region Fields

        private Vector3 _movement;
        private bool _movementLocked;
        private bool _sprintLocked;

        

        #endregion


        #region MonoBehaviour
        private void Update()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (_movementLocked)
                return;
            if(vertical==0 && horizontal==0)
            {
                animator.SetFloat("Horizontal", _lastHorisontalInput/2);
                animator.SetFloat("Vertical", _lastVerticalInput/2);
                
            }
            else if ((Math.Abs(vertical)>0.5f) | (Math.Abs(horizontal)>0.5f))
            {
                animator.SetFloat("Horizontal", horizontal);
                animator.SetFloat("Vertical", vertical);
                _lastHorisontalInput=horizontal;
                _lastVerticalInput=vertical;
                
            }
           
            /* animator.SetFloat("Horizontal", horizontal);
            animator.SetFloat("Vertical", vertical); */
            animator.SetFloat("Speed", _movement.sqrMagnitude);
            _movement = (transform.right * horizontal + transform.forward * vertical).normalized;
        }

        private void FixedUpdate()
        {
            if (_movementLocked)
                return;

            float sprint = _sprintLocked == false && Input.GetKey(KeyCode.LeftShift) ? sprintSpeedBonus : 0;
            physicsBody.velocity = _movement * (speed + sprint) * Time.fixedDeltaTime;
            physicsBody.AddForce(Physics.gravity * physicsBody.mass * GravityMultiplier);
        }

        public void SetSprint(bool sprint) => _sprintLocked = !sprint;

        public void SetWalk(bool walk) 
        { 
            _movementLocked = !walk;
            physicsBody.velocity = walk == false ? Vector2.zero : physicsBody.velocity;
            GetComponent<Animator>().enabled = walk;
        }

        public void TurnOffMovement()
        {
            GetComponent<Activator>().enabled = false;
            //this.enabled = false;
            _movementLocked=true;
            animator.SetFloat("Horizontal", _lastHorisontalInput/2 );
            animator.SetFloat("Vertical", _lastVerticalInput/2);
            physicsBody.velocity =Vector3.zero;
        }
        
        public void TurnOnMovement()
        {
            GetComponent<Activator>().enabled = true;
            _movementLocked=false;
            //this.enabled = true;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Slope")
            {
                speed = speed * 2;
            }
        }
        public void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Slope")
            {
                speed = speed /2;
            }
        }

        #endregion
    }
}
