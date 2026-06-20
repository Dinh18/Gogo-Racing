using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour, ICarInput
{
    public float MoveInput {get; private set;}

    public float TurnInput {get; private set;}

    public bool IsDrifting {get; private set;}

    public bool IsUsingItem {get; private set;}

    public bool IsBoosting {get; private set;}
    
    private CarProgress carProgress;
    private float finishedTimer = 0f;
    public bool isStop;
    [SerializeField] private float extraDriveTime = 2f; // Thời gian cho phép lái thêm sau khi về đích

    void Start()
    {
        carProgress = this.GetComponent<CarProgress>();
        isStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (carProgress == null) carProgress = this.GetComponent<CarProgress>();
        if(isStop) return;
        
        bool isCarFinished = carProgress != null && carProgress.IsFinished();
        if (isCarFinished)
        {
            finishedTimer += Time.deltaTime;
        }

        RaceState currState = RaceManager.Instance.GetCurrState();
        bool isGameStateAllowingInput = (currState == RaceState.Racing);

        // Khóa input nếu không ở state cho phép hoặc đã hết thời gian chạy thêm
        if(!isGameStateAllowingInput || (isCarFinished && finishedTimer >= extraDriveTime)) 
        {
            MoveInput = 0;
            TurnInput = 0;
            IsDrifting = false;
            IsUsingItem = false;
            IsBoosting = false;
            return;
        }   

        // Vẫn nhận input nếu chưa khóa
        if (isCarFinished) 
        {
            // Tùy chọn: Ghi đè tự động ga nhẹ khi về đích nếu không muốn người chơi phải giữ nút
            // MoveInput = 1f; 
        }

        MoveInput = Input.GetAxis("Vertical");
        TurnInput = Input.GetAxis("Horizontal");
        IsDrifting = Input.GetKey(KeyCode.LeftControl);
        IsUsingItem = Input.GetKeyDown(KeyCode.U); // Phải dùng GetKeyDown để tránh việc sử dụng nhiều item cùng lúc khi lỡ đè phím
        IsBoosting = Input.GetKey(KeyCode.Space);
    }

    public void ResetInput()
    {
        MoveInput = 0;
            TurnInput = 0;
            IsDrifting = false;
            IsUsingItem = false;
            IsBoosting = false;
    }
}
