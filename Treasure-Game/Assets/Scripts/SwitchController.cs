using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class SwitchController : MonoBehaviour
{
    public List<Interactee> OnPressActions;
    public bool singleTrigger = false;

    private Subject switchObserver;
    private static Interactor interactor;

    private bool isPressed;

    public interface ISubject
    {
        void NotifyIfPress(bool state);
    }

    public class Subject : ISubject
    {
        Interactor interactor;
        private List<Interactee> switchPressObservers = new List<Interactee>();
        public Subject(List<Interactee> OnPressActions, Interactor interactor)
        {
            switchPressObservers = OnPressActions;
            this.interactor = interactor;
        }

        public void NotifyIfPress(bool state)
        {
            foreach (var observer in switchPressObservers)
            {
                interactor.Interact(observer, state ? 0 : 1);
            }
        }
    }

    private void Start()
    {
        interactor = GetComponent<Interactor>();
        switchObserver = new Subject(OnPressActions, interactor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            this.transform.position += -Vector3.up * .2f;
            switchObserver.NotifyIfPress(true);
        }
        isPressed = true;

    }

    private void OnTriggerExit(Collider other)
    {
        if (!singleTrigger)
        {
            if (isPressed)
            {
                this.transform.position += Vector3.up * .2f;
                switchObserver.NotifyIfPress(false);
            }
            isPressed = false;
        }

    }
}
