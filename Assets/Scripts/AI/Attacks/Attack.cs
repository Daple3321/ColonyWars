[System.Serializable]
public abstract class Attack
{
    protected Unit owner;
    protected AttackData attackData;
    protected Affiliation affiliation;
    public Attack(Unit owner, AttackData attackData, Affiliation affiliation){
        this.owner = owner;
        this.attackData = attackData;
        this.affiliation = affiliation;
    }
    
    public bool attackFinished = false;
    public abstract void OneShotAttack();
    public abstract void ConstantAttack(); // ужасное название
}
