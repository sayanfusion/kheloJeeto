using UnityEngine;
using TMPro;
using PlayFab.MultiplayerModels;
using UnityEngine.EventSystems;

[System.Serializable]
public enum BetType
{
    Straight,
    Split,
    Corner,
    Street,
    DoubleStreet,
    Row,
    Dozen,
    Low,
    High,
    Even,
    Odd,
    Red,
    Black
}

public class BetSpace : MonoBehaviour
{
    public int couterForBackend = 0;
    public int count = 0;
    public int count1 = 0;
    public int chipValue;
    public int chip0, chip1, chip2, chip3, chip4, chip5, chip6, chip7, chip8, chip9, chip10, chip11, chip12, chip13, chip14, chip15, chip16, chip17, chip18, chip19
        , chip20, chip21, chip22, chip23, chip24, chip25, chip26, chip27, chip28, chip29, chip30, chip31, chip32, chip33, chip34, chip35, chip36, chip37;

    public int t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13,
        t14, t15, t16, t17, t18, t19, t20, t21, t22, t23, t24, t25, t26, t27, t28, t29, t30, t31, t32, t33, t34, t35, t36, t37;
    public int chipId;

    public ChipStack stack;
    public BetType betType;
    public static int numLenght = 36; //Change this to change the amount of rewards
    public TMP_Text resultText;
    [SerializeField]
    public int[] winningNumbers;
    public static int counter = 1;
    public static bool isPressing = false;
    public static bool isDoublePressed = false;


    public MeshRenderer[] betSpaceRender;


    public MeshRenderer mesh;
    private int lastBet = 0;
    public static BetSpace Instance;
    public static bool BetsEnabled { get; private set; } = true;

    public int GetValue() => stack.GetValue();
    public bool IsRebet;
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    void Start()
    {
        IsRebet = false;
        count = 0;
        count1 = 0;
        mesh = GetComponent<MeshRenderer>();

        if (mesh)
            mesh.enabled = false;

        stack = Cloth.InstanceStack();
        stack.SetInitialPosition(transform.position);
        stack.transform.SetParent(transform);
        stack.transform.localPosition = Vector3.zero;
        ResultManager.RegisterBetSpace(this);
        //AmericanWheel.OnRebetAndSpin += Rebet;
    }

    private void OnRightMouseButtonClick()
    {
        //Debug.Log("right click............................................................." + this.gameObject.name);
        int selectedValue = ChipManager.GetSelectedValue();
        Debug.Log("Remove selected value: " + selectedValue);
        RemoveBet(selectedValue);
        //ToolTipManager.SelectTarget(stack);
        //ToolTipManager.Deselect();
        mousetest.rightclick = false;
    }


    void Update()
    {


        if (SceneRoulette.isClearPressed == true)
        {
            count = 0;
            count1 = 0;
        }

        if (Input.GetMouseButtonDown(1) && mousetest.rightclick == true && timeCounter.instance.secondleft > 5)
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                //Debug.Log("selected objecct: " + objectHit.name);
                objectHit.GetComponent<BetSpace>().OnRightMouseButtonClick();
            }
            mousetest.rightclick = false;
        }



            if (SceneRoulette.numDisplayed == true && count1 > 0 && couterForBackend == 0)
        {
            //////////////////WinSequence.Instance.getuserdetails();
            couterForBackend += 1;
            Invoke("CounterReset", 5);
        }

        if (SceneRoulette.numDisplayed == true)
        {
            Invoke("meshDisabler", 8);
            count = 0;
            count1 = 0;

        }
       
    }

    public void CounterReset()
    {
        couterForBackend = 0;
    }

    ////////////////private void OnMouseEnter()
    ////////////////{
    ////////////////    ToolTipManager.SelectTarget(stack);

    ////////////////    if (mesh)
    ////////////////        mesh.enabled = true;

    ////////////////    if (!BetsEnabled)
    ////////////////        return;


    ////////////////    if (betSpaceRender.Length > 0)
    ////////////////    {
    ////////////////        foreach (MeshRenderer spaceRender in betSpaceRender)
    ////////////////        {
    ////////////////            spaceRender.enabled = true;
    ////////////////        }
    ////////////////    }
    ////////////////}

    ////////////////void OnMouseExit()
    ////////////////{
    ////////////////    ToolTipManager.Deselect();

    ////////////////    if (mesh)
    ////////////////        mesh.enabled = false;

    ////////////////    if (!BetsEnabled)
    ////////////////        return;

    ////////////////    if (betSpaceRender.Length > 0)
    ////////////////    {
    ////////////////        foreach (MeshRenderer spaceRender in betSpaceRender)
    ////////////////        {
    ////////////////            spaceRender.enabled = false;
    ////////////////        }
    ////////////////    }
    ////////////////}

    private void OnMouseUp()
    {
        int selectedValue = ChipManager.GetSelectedValue();
        Debug.LogError("isDoublePressed " + isDoublePressed);
        Debug.LogError("EventSystem.current.IsPointerOverGameObject " + EventSystem.current.IsPointerOverGameObject());
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (chipId == 0)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip0 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip0 += t0;
                        chipValue = chip0;
                    }
                    else
                    {
                        chipValue = chip0;
                    }
                    count1 += 1;

                }


                if (count > 0)
                {
                    IsRebet = false;
                    chip0 += selectedValue;
                    chipValue = chip0;
                    t0 = chipValue;
                }
            }
        }

        if (chipId == 1)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {

                    chip1 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip1 += t1;
                        chipValue = chip1;
                    }
                    else
                    {
                        chipValue = chip1;
                    }

                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip1 += selectedValue;
                    chipValue = chip1;
                    t1 = chipValue;
                }
            }
        }

        if (chipId == 2)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip2 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip2 += t1;
                        chipValue = chip2;
                    }
                    else
                    {
                        chipValue = chip2;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip2 += selectedValue;
                    chipValue = chip2;
                    t2 = chipValue;
                }
            }
        }

        if (chipId == 3)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip3 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip3 += t3;
                        chipValue = chip3;
                    }
                    else
                    {
                        chipValue = chip3;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip3 += selectedValue;
                    chipValue = chip3;
                    t3 = chipValue;
                }
            }
        }
        if (chipId == 4)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip4 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip4 += t4;
                        chipValue = chip4;
                    }
                    else
                    {
                        chipValue = chip4;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip4 += selectedValue;
                    chipValue = chip4;
                    t4 = chipValue;
                }
            }
        }
        if (chipId == 5)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip5 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip5 += t5;
                        chipValue = chip5;
                    }
                    else
                    {
                        chipValue = chip5;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip5 += selectedValue;
                    chipValue = chip5;
                    t5 = chipValue;
                }
            }
        }

        if (chipId == 6)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip6 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip6 += t6;
                        chipValue = chip6;
                    }
                    else
                    {
                        chipValue = chip6;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip6 += selectedValue;
                    chipValue = chip6;
                    t6 = chipValue;
                }
            }
        }

        if (chipId == 7)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip7 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip7 += t7;
                        chipValue = chip7;
                    }
                    else
                    {
                        chipValue = chip7;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip7 += selectedValue;
                    chipValue = chip7;
                    t7 = chipValue;
                }
            }
        }

        if (chipId == 8)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip8 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip8 += t8;
                        chipValue = chip8;
                    }
                    else
                    {
                        chipValue = chip8;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip8 += selectedValue;
                    chipValue = chip8;
                    t8 = chipValue;
                }
            }
        }
        if (chipId == 9)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip9 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip9 += t9;
                        chipValue = chip9;
                    }
                    else
                    {
                        chipValue = chip9;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip9 += selectedValue;
                    chipValue = chip9;
                    t9 = chipValue;
                }
            }
        }
        if (chipId == 10)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip10 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip10 += t10;
                        chipValue = chip10;
                    }
                    else
                    {
                        chipValue = chip10;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip10 += selectedValue;
                    chipValue = chip10;
                    t10 = chipValue;
                }
            }
        }

        if (chipId == 11)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip11 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip11 += t11;
                        chipValue = chip11;
                    }
                    else
                    {
                        chipValue = chip11;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip11 += selectedValue;
                    chipValue = chip11;
                    t11 = chipValue;
                }
            }
        }

        if (chipId == 12)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip12 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip12 += t12;
                        chipValue = chip12;
                    }
                    else
                    {
                        chipValue = chip12;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip12 += selectedValue;
                    chipValue = chip12;
                    t12 = chipValue;
                }
            }
        }

        if (chipId == 13)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip13 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip13 += t13;
                        chipValue = chip13;
                    }
                    else
                    {
                        chipValue = chip13;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip13 += selectedValue;
                    chipValue = chip13;
                    t13 = chipValue;
                }
            }
        }

        if (chipId == 14)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip14 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip14 += t14;
                        chipValue = chip14;
                    }
                    else
                    {
                        chipValue = chip14;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip14 += selectedValue;
                    chipValue = chip14;
                    t14 = chipValue;
                }
            }
        }

        if (chipId == 15)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip15 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip15 += t15;
                        chipValue = chip15;
                    }
                    else
                    {
                        chipValue = chip15;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip15 += selectedValue;
                    chipValue = chip15;
                    t15 = chipValue;
                }
            }
        }

        if (chipId == 16)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip16 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip16 += t16;
                        chipValue = chip16;
                    }
                    else
                    {
                        chipValue = chip16;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip16 += selectedValue;
                    chipValue = chip16;
                    t16 = chipValue;
                }
            }
        }

        if (chipId == 17)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip17 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip17 += t17;
                        chipValue = chip17;
                    }
                    else
                    {
                        chipValue = chip17;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip17 += selectedValue;
                    chipValue = chip17;
                    t17 = chipValue;
                }
            }
        }

        if (chipId == 18)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip18 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip18 += t18;
                        chipValue = chip18;
                    }
                    else
                    {
                        chipValue = chip18;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip18 += selectedValue;
                    chipValue = chip18;
                    t18 = chipValue;
                }
            }
        }

        if (chipId == 19)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip19 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip19 += t19;
                        chipValue = chip19;
                    }
                    else
                    {
                        chipValue = chip19;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip19 += selectedValue;
                    chipValue = chip19;
                    t19 = chipValue;
                }
            }
        }


        if (chipId == 20)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip20 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip20 += t20;
                        chipValue = chip20;
                    }
                    else
                    {
                        chipValue = chip20;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip20 += selectedValue;
                    chipValue = chip20;
                    t20 = chipValue;
                }
            }
        }

        if (chipId == 21)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip21 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip21 += t21;
                        chipValue = chip21;
                    }
                    else
                    {
                        chipValue = chip21;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip21 += selectedValue;
                    chipValue = chip21;
                    t21 = chipValue;
                }
            }
        }

        if (chipId == 22)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip22 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip22 += t22;
                        chipValue = chip22;
                    }
                    else
                    {
                        chipValue = chip22;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip22 += selectedValue;
                    chipValue = chip22;
                    t22 = chipValue;
                }
            }
        }

        if (chipId == 23)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip23 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip23 += t23;
                        chipValue = chip23;
                    }
                    else
                    {
                        chipValue = chip23;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip23 += selectedValue;
                    chipValue = chip23;
                    t23 = chipValue;
                }
            }
        }

        if (chipId == 24)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip24 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip24 += t24;
                        chipValue = chip24;
                    }
                    else
                    {
                        chipValue = chip24;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip24 += selectedValue;
                    chipValue = chip24;
                    t24 = chipValue;
                }
            }
        }

        if (chipId == 25)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip25 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip25 += t25;
                        chipValue = chip25;
                    }
                    else
                    {
                        chipValue = chip25;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip25 += selectedValue;
                    chipValue = chip25;
                    t25 = chipValue;
                }
            }
        }

        if (chipId == 26)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip26 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip26 += t26;
                        chipValue = chip26;
                    }
                    else
                    {
                        chipValue = chip26;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip26 += selectedValue;
                    chipValue = chip26;
                    t26 = chipValue;
                }
            }
        }

        if (chipId == 27)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip27 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip27 += t27;
                        chipValue = chip27;
                    }
                    else
                    {
                        chipValue = chip27;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip27 += selectedValue;
                    chipValue = chip27;
                    t27 = chipValue;
                }
            }
        }

        if (chipId == 28)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip28 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip28 += t28;
                        chipValue = chip28;
                    }
                    else
                    {
                        chipValue = chip28;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip28 += selectedValue;
                    chipValue = chip28;
                    t28 = chipValue;
                }
            }
        }

        if (chipId == 29)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip29 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip29 += t29;
                        chipValue = chip29;
                    }
                    else
                    {
                        chipValue = chip29;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip29 += selectedValue;
                    chipValue = chip29;
                    t29 = chipValue;
                }
            }
        }

        if (chipId == 30)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip30 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip30 += t30;
                        chipValue = chip30;
                    }
                    else
                    {
                        chipValue = chip30;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip30 += selectedValue;
                    chipValue = chip30;
                    t30 = chipValue;
                }
            }
        }


        if (chipId == 31)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip31 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip31 += t31;
                        chipValue = chip31;
                    }
                    else
                    {
                        chipValue = chip31;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip31 += selectedValue;
                    chipValue = chip31;
                    t31 = chipValue;
                }
            }
        }


        if (chipId == 32)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip32 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip32 += t32;
                        chipValue = chip32;
                    }
                    else
                    {
                        chipValue = chip32;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip32 += selectedValue;
                    chipValue = chip32;
                    t32 = chipValue;
                }
            }
        }

        if (chipId == 33)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip33 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip33 += t33;
                        chipValue = chip33;
                    }
                    else
                    {
                        chipValue = chip33;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip33 += selectedValue;
                    chipValue = chip33;
                    t33 = chipValue;
                }
            }
        }

        if (chipId == 34)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip34 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip34 += t34;
                        chipValue = chip34;
                    }
                    else
                    {
                        chipValue = chip34;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip34 += selectedValue;
                    chipValue = chip34;
                    t34 = chipValue;
                }
            }
        }

        if (chipId == 35)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip35 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip35 += t35;
                        chipValue = chip35;
                    }
                    else
                    {
                        chipValue = chip35;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip35 += selectedValue;
                    chipValue = chip35;
                    t35 = chipValue;
                }
            }
        }


        if (chipId == 36)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip36 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip36 += t36;
                        chipValue = chip36;
                    }
                    else
                    {
                        chipValue = chip36;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip36 += selectedValue;
                    chipValue = chip36;
                    t36 = chipValue;
                }
            }
        }

        if (chipId == 37)
        {
            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0 && ChipManager.selected)
            {
                if (count1 == 0)
                {
                    chip37 = ChipManager.GetSelectedValue();
                    if (IsRebet)
                    {
                        chip37 += t37;
                        chipValue = chip37;
                    }
                    else
                    {
                        chipValue = chip37;
                    }
                    count1 += 1;

                }

                if (count > 0)
                {
                    IsRebet = false;
                    chip37 += selectedValue;
                    chipValue = chip37;
                    t37 = chipValue;
                }
            }
        }

        ApplyBet(selectedValue);


        ToolTipManager.SelectTarget(stack);
        if (timer.betallowerd == true)
        {
            if (mesh)
            {
                mesh.enabled = true;
                SceneRoulette._Instance.rebetButton.interactable = false;
            }
            if (!BetsEnabled)
                return;


            if (betSpaceRender.Length > 0)
            {
                foreach (MeshRenderer spaceRender in betSpaceRender)
                {
                    spaceRender.enabled = true;
                }
            }

            Invoke("meshDisabler", 1);
        }

    }



    public void DoubleBet()
    {

        ////////////////////if (timer.betallowerd == true)
        ////////////////////{
        ////////////////////    if (!LimitBetPlate.AllowLimit(selValue))
        ////////////////////    {
        ////////////////////        return;
        ////////////////////    }

        ////////////////////    if (BetsEnabled && selValue * 2 > 0 && BalanceManager.Balance - selValue >= 0)
        ////////////////////    {

        ////////////////////        AudioManager.SoundPlay(3);
        ////////////////////        BalanceManager.ChangeBalance(-selValue);
        ////////////////////        ResultManager.totalBet += selValue;
        ////////////////////        stack.Add(selValue * 2);

        ////////////////////        lastBet = stack.GetValue();

        ////////////////////        BetPool.Instance.Add(this, selValue * 2);

        ////////////////////        SceneRoulette._Instance.clearButton.interactable = true;
        ////////////////////        SceneRoulette._Instance.undoButton.interactable = true;
        ////////////////////        SceneRoulette._Instance.rollButton.interactable = true;
        ////////////////////        ////////////////////////SceneRoulette._Instance.rebetButton.interactable = true;
        ////////////////////        /////////////////////////SceneRoulette._Instance.rebetButton.gameObject.SetActive(false);
        ////////////////////        SceneRoulette.UpdateLocalPlayerText();
        ////////////////////    }
        ////////////////////}


        int sel = ResultManager.totalBet;
       // isPressing = true;
        isDoublePressed = true;
        
        if (timer.betallowerd == true)
        {
            if (!LimitBetPlate.AllowLimit(sel))
                return;

            if (BetsEnabled && sel > 0 && BalanceManager.Balance - sel >= 0)
            {
                counter = counter * 2;
                Debug.Log("ccccccccccccccccccccccccccccccccc" + counter);
                AudioManager.SoundPlay(3);
                print("Bet applyed! with: " + sel);

                BalanceManager.ChangeBalance(-sel);
                ResultManager.totalBet += sel;
                Debug.Log($"total bet:{sel}");
                for (int i = 0; i < UIController.BetSpace.Count; i++)
                {
                    UIController.BetSpace[i].stack.Add2(chipId);
                }
               // stack.Add(sel);

                lastBet = stack.GetValue();

                BetPool.Instance.Add(this, sel);

                SceneRoulette._Instance.clearButton.interactable = true;
                SceneRoulette._Instance.undoButton.interactable = true;
                SceneRoulette._Instance.rollButton.interactable = true;
                //////////////////////SceneRoulette._Instance.rebetButton.interactable =true;
                /////////////////////////SceneRoulette._Instance.rebetButton.gameObject.SetActive(false);
                SceneRoulette.UpdateLocalPlayerText();
            }
        }
        /////////////////ApplyBet(sel);


    }

    public void ApplyBet(int selectedValue)
    {
        //////////////isPressing = false;

        isDoublePressed = false;
        if (timer.betallowerd == true)
        {
            if (!LimitBetPlate.AllowLimit(selectedValue))
                return;

            if (BetsEnabled && selectedValue > 0 && BalanceManager.Balance - selectedValue >= 0)
            {

                count += 1;
                AudioManager.SoundPlay(3);
                print("Bet applyed! with: " + selectedValue);



                BalanceManager.ChangeBalance(-selectedValue);
                ResultManager.totalBet += selectedValue;
                stack.Add(chipId,selectedValue);
                //Debug.LogError("selectedValue " + selectedValue);
                lastBet += selectedValue;
                //Debug.LogError("lastBet apply " + lastBet);
                BetPool.Instance.Add(this, selectedValue);

                SceneRoulette._Instance.clearButton.interactable = true;
                SceneRoulette._Instance.undoButton.interactable = true;
                SceneRoulette._Instance.rollButton.interactable = true;
                //////////////////////SceneRoulette._Instance.rebetButton.interactable =true;
                /////////////////////////SceneRoulette._Instance.rebetButton.gameObject.SetActive(false);
                SceneRoulette.UpdateLocalPlayerText();
            }
        }
    }

    public void RemoveBet(int value)
    {
        Debug.Log("Last bet: "+lastBet);
        
        if (lastBet > 0)
        {
            if (ResultManager.totalBet > 0)
            {
                BalanceManager.ChangeBalance(value);
                SceneRoulette._Instance._AmeWheel.SetValue(chipId, -value);
                ResultManager.totalBet -= value;
                //stack.Remove(value);
                lastBet = stack.GetValue();
                Debug.Log("chipId "+ transform.name);
 
                switch (transform.name)
                {
                    case "high7":
                        Debug.Log("chip7 "+ chip7);
                        stack.SetValue(chipId, chip7);
                        break;
                    default:
                        break;

                }
                //Debug.Log("ResultManager.totalBet " + ResultManager.totalBet);
                //chipValue = ResultManager.totalBet;
                Debug.Log("Last bet: "+lastBet);
                lastBet-=value; 
                stack.Remove(chipId, lastBet);
                //lastBet = stack.Clear(); 
                SceneRoulette.UpdateLocalPlayerText();
            }
        }
        //if (lastBet == 0)
        //{
        //    count = 0;
        //}
    }

    public int ResolveBet(int result)
    {
        int multiplier = numLenght / winningNumbers.Length;

        Debug.LogError("multiplier "+ multiplier);
        bool won = false;

        foreach (int num in winningNumbers)
        {
            if (num == result)
            {
                won = true;

                if (mesh && betType == BetType.Straight)
                    mesh.enabled = true;
                break;
            }
        }

        int winAmount = 0;


        if (won)
        {
            winAmount = stack.Win(multiplier);
        }
        else
        {
            stack.Clear();
        }

        return winAmount;
    }

    public void Rebet()
    {
        try
        {
            Debug.LogError("rebet " + lastBet);
            if (lastBet == 0)
                return;

            if (!LimitBetPlate.AllowLimit(lastBet))
            {
                lastBet = 0;
                return;
            }


            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0)
            {
                Debug.LogError("lastBet " + lastBet);
                BetSpace.counter = SceneRoulette.counter1;
                BalanceManager.ChangeBalance(-lastBet);
                ResultManager.totalBet += lastBet;
                stack.SetValue(chipId, lastBet);
                lastBet = stack.GetValue();
                IsRebet = true;
                BetPool.Instance.Add(this, lastBet);

                SceneRoulette._Instance.clearButton.interactable = true;
                SceneRoulette._Instance.undoButton.interactable = true;
                SceneRoulette._Instance.rollButton.interactable = true;
                //////////////////////////////////SceneRoulette._Instance.rebetButton.interactable = true;
                /////////////////////////SceneRoulette._Instance.rebetButton.gameObject.SetActive(false);
                SceneRoulette.UpdateLocalPlayerText();
                //lastBet = 0;
            }
            else
                lastBet = 0;
        }
        catch { }
    }

    public void DoubleRebet()
    {
        try
        {
            if (lastBet == 0)
                return;

            if (!LimitBetPlate.AllowLimit(lastBet))
            {
                lastBet = 0;
                return;
            }

            if (BetsEnabled && BalanceManager.Balance - lastBet >= 0)
            {
                int last2Bet = 2 * lastBet;

                BalanceManager.ChangeBalance(-last2Bet);
                ResultManager.totalBet += last2Bet;
                stack.SetValue(chipId, last2Bet);
                last2Bet = stack.GetValue();

                BetPool.Instance.Add(this, last2Bet);

                SceneRoulette._Instance.clearButton.interactable = true;
                SceneRoulette._Instance.undoButton.interactable = true;
                SceneRoulette._Instance.rollButton.interactable = true;
                //////////////////////////////SceneRoulette._Instance.rebetButton.interactable = true;
                //////////////////////////SceneRoulette._Instance.rebetButton.gameObject.SetActive(false);
                SceneRoulette.UpdateLocalPlayerText();
            }
            else
                lastBet = 0;
        }
        catch { }
    }

    public void Clear()
    {
        try
        {
            int val = stack.GetValue();
            BalanceManager.ChangeBalance(val);
            ResultManager.totalBet -= val;
            //lastBet = 0;

            stack.Clear();
            SceneRoulette.UpdateLocalPlayerText();
            ToolTipManager.Deselect();

            if (mesh)
                mesh.enabled = false;

            if (!BetsEnabled)
                return;

            if (betSpaceRender.Length > 0)
            {
                foreach (MeshRenderer spaceRender in betSpaceRender)
                {
                    spaceRender.enabled = false;
                }
            }
        }
        catch { }
    }

    public void meshDisabler()
    {
        if (mesh)
        {
            mesh.enabled = false;
        }

        if (betSpaceRender.Length > 0)
        {
            foreach (MeshRenderer spaceRender in betSpaceRender)
            {
                spaceRender.enabled = false;
            }
        }

    }

    public static void EnableBets(bool enable)
    {
        BetsEnabled = enable;
    }

    public void CountReset()
    {


    }




}