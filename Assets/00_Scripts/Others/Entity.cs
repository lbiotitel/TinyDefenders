using UnityEngine;

public class Entity : MonoBehaviour
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
