using Assets;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarNeuralCoreBase : MonoBehaviour, IPointerClickHandler
{
    public delegate void CarNeuralCoreEventHandler (CarNeuralCoreBase carNeuralCore);
    public event CarNeuralCoreEventHandler OnCarClicked;
    public event CarNeuralCoreEventHandler OnExplode;

    [SerializeField] protected CarTelemetry carTelemetry;
    [SerializeField] protected CarRadar carRadar;
    [SerializeField] protected CarController carController;
    [SerializeField] protected CarFitness carFitness;
    [SerializeField] protected CarParticlesManager carParticlesManager;
    [SerializeField] protected new Rigidbody rigidbody;
    [SerializeField] protected Explosion explosion;

    protected NeuralNetwork neuralNetwork;
    protected NetworkTopologySimpleData networkTopology = new NetworkTopologySimpleData ();

    public float AngleBetweenSensors
    {
        get;
        set;
    }

    public float SensorsLength
    {
        get;
        set;
    }

    public bool IsActive
    {
        get;
        set;
    }

    public bool ExplodeOnDisable
    {
        get;
        set;
    }

    protected void Awake ()
    {
        carFitness.OnWallHit += onWallHit;
        carFitness.OnGatePassed += onGatePassed;
        carFitness.OnFinalGatePassed += onFinalGatePassed;
    }

    public void SetWeights (double [] weights)
    {
        neuralNetwork.SetWeights (weights);
    }

    public void SetSensorsVisible (bool visible)
    {
        carRadar.SetSensorsVisible (visible);
    }

    public void SetSensorsLength (float length)
    {
        this.SensorsLength = length;
        carRadar.SetSensorsLength (length);
    }

    public void SetAngleBetweenSensors (float angle)
    {
        this.AngleBetweenSensors = angle;
        carRadar.SetAngleBetweenSensors (angle);
    }

    public CarSimpleData GetCarSimpleData ()
    {
        CarFitness carFitness = GetComponent<CarFitness> ();
        return new CarSimpleData (GetWeights (), carFitness.Fitness, carFitness.DistanceTravelled, SensorsLength, AngleBetweenSensors);
    }

    public double [] GetWeights ()
    {
        return neuralNetwork.GetWeights ();
    }

    public virtual void Init (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            initNeuralNetwork (networkTopology);
            carRadar.Init (networkTopology.SensorsCount, AngleBetweenSensors, SensorsLength);
        }
    }

    protected void explode ()
    {
        explosion.Explode ();
        OnExplode?.Invoke (this);
    }

    void initNeuralNetwork (NetworkTopologySimpleData networkTopology)
    {
        if (networkTopology != null)
        {
            this.networkTopology = networkTopology.GetCopy ();
            int outputCount = 1 + (this.networkTopology.TorqueOutput ? 1 : 0) 
                + (this.networkTopology.HandbrakeOutput ? 1 : 0); //there is always one output - steer angle
            int additionalInputCount = (this.networkTopology.TorqueInput ? 1 : 0) + (this.networkTopology.VelocityInput ? 1 : 0) +
                (this.networkTopology.SteerAngleInput ? 1 : 0) + (this.networkTopology.MovementAngleInput ? 1 : 0);
            neuralNetwork = new NeuralNetwork (networkTopology.SensorsCount + additionalInputCount, this.networkTopology.HiddenLayerNeuronsCount, outputCount);
        }
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        OnCarClicked?.Invoke (this);
    }

    protected virtual void onWallHit () { }

    protected virtual void onGatePassed (int gateIndex) { }

    protected virtual void onFinalGatePassed (int gateIndex) {}
}
