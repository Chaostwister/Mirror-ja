public class Gun : Weapon
{
    public override void OnLeftClick()
    {
        print("leftClicked with gun");
    }

    public override void OnRightClick()
    {
        throw new System.NotImplementedException();
    }
}