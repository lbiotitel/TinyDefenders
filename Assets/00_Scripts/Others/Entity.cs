using Unity.Netcode;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void GetInitEntity(string controller,string rarity)
    {
        animator.runtimeAnimatorController = Resources.Load<HeroScriptable>("HeroDatas/" + rarity + "/" + controller).runtimeAnimatorController;
    }

    public void SortingOrderChange(int value)
    {
        spriteRenderer.sortingOrder = value;
    }
       
    protected void AnimatorChange(string name, bool trigger)
    {
        if (trigger)
        {
            animator.SetTrigger(name);
            return;
        }

        animator.SetBool("IDLE", false);
        animator.SetBool("MOVE", false);
        animator.SetBool(name, true);
    }
}
