public class Resource : Item
{
    public override void OnLeftClick()
    {
        print("this a resource left click");
    }

    public override void OnRightClick()
    {
        throw new System.NotImplementedException();
    }
}