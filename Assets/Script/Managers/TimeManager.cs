using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("时间设置")]
    [SerializeField] private float defaultTimeScale = 1.0f;
    [SerializeField] private float bulletTimeScale = 0.1f;
    [SerializeField] private float transitionDuration = 0.2f; // 如果需要，可以进行平滑过渡

    private float _targetTimeScale;
    private float _velocity; // 用于 Mathf.SmoothDamp

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 如果我们需要按场景独立存在，则不使用 DontDestroyOnLoad，但通常 TimeManager 是全局的
            // 对于本次项目规模来说，简单的单例模式就够了
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _targetTimeScale = defaultTimeScale;
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void EnterBulletTime()
    {
        _targetTimeScale = bulletTimeScale;
        ApplyTimeScale();
    }

    public void ExitBulletTime()
    {
        _targetTimeScale = defaultTimeScale;
        ApplyTimeScale();
    }

    private void ApplyTimeScale()
    {
        Time.timeScale = _targetTimeScale;
        // 调整 fixedDeltaTime，使物理模拟与时间缩放保持一致
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
