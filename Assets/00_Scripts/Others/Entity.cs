using Unity.Netcode;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected void AnimatorChange(string name, bool trigger)
    {
        if (trigger)
        {
            animator.SetTrigger(name);
        }
        else{
            animator.SetBool(name,true);
        }
    }
}
