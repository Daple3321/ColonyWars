public static class DragDropManager
{
    public static InventorySlot currentlyDraggedSlot = null;
    public static int dragQuantity = -1;

    public static void Init()
    {
        currentlyDraggedSlot = null;
        dragQuantity = -1;
    }
}
