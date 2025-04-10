public interface IBattleAction
{
    void Execute(BattleUnit source, BattleUnit target, int power, int duration);
    bool IsValidTarget(BattleUnit source, BattleUnit target);
}