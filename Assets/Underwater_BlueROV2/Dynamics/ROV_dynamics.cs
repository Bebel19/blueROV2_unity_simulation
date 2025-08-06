using UnityEngine;

public class ROV_dynamics : MonoBehaviour
{
    public Thruster Thruster_tau; // Reference to the thrust vector provider

    // Mass and hydrodynamic parameters
    private float[] M_RB = new float[6]; // Rigid-body mass and inertia
    private float[] M_A = new float[6];  // Added mass (hydrodynamic)
    private float[] M = new float[6];    // Total effective mass

    // Forces and torques
    public float[] tau;                      // Control input (thrust)
    private float[] C_nu = new float[6];     // Coriolis and centripetal
    private float[] D_O = new float[6];      // Linear drag
    private float[] D_N = new float[6];      // Nonlinear drag
    private float[] D = new float[6];        // Total drag
    public float[] G;         // Gravity + Buoyancy

    // States
    public float[] nu_now;        // Linear & angular velocity
    public float[] nu_now_dot;    // Acceleration
    public float[] eta_now;       // Position and orientation

    // Environment
    public float X_dist_vel;
    public float Y_dist_vel;
    public float dt;

    // Internals
    public float[] dist_vel;                     // Disturbance velocity (body frame)
    float[] World_dist;                          // Disturbance velocity (world frame)
    private float[,] Jacv_1;                     // Rotation matrix from body to world
    private float[] I_c = new float[3];          // Inertia moments

    private float mass = 13.5f;
    private float Volume = 0.0135f;
    private float rho = 1000.0f;
    private float gravity = 9.82f;
    private float z_b = -0.02f;//-0.01f;            //Restoring force

    private float W, B;

    private Vector3 pos_buf, angle_buf;

    void Start()
    {
        G = new float[6];
        Jacv_1 = new float[3,3];
        nu_now = new float[6];
        nu_now_dot = new float[6];
        eta_now = new float[6];
        dist_vel = new float[6];
        World_dist = new float[6];
        tau = new float[6];
        
        
        // Initialize inertia
        I_c[0] = 0.26f; I_c[1] = 0.23f; I_c[2] = 0.37f;
        for (int i = 0; i < 6; i++) { tau[i] = 0.0f; nu_now[i] = 0.0f; }

        // Rigid-body mass matrix
        M_RB[0] = mass; M_RB[1] = mass; M_RB[2] = mass;
        M_RB[3] = I_c[0]; M_RB[4] = I_c[1]; M_RB[5] = I_c[2];

        // Added mass
        M_A[0] = 6.3567f; M_A[1] = 7.1206f; M_A[2] = 18.6863f;
        M_A[3] = 0.1858f; M_A[4] = 0.1348f; M_A[5] = 0.2215f;

        for (int i = 0; i < 6; i++) M[i] = M_RB[i] + M_A[i];

        // Linear drag coefficients
        D_O[0] = 141.0f; D_O[1] = 217.0f; D_O[2] = 190.0f;
        D_O[3] = 1.192f; D_O[4] = 0.47f; D_O[5] = 1.5f;

        // Initial position and orientation (in robot coordinates)
        eta_now[0] = transform.position.z;
        eta_now[1] = transform.position.x;
        eta_now[2] = 200.0f - transform.position.y;
        eta_now[3] = -Mathf.Deg2Rad * transform.eulerAngles.z;
        eta_now[4] = -Mathf.Deg2Rad * transform.eulerAngles.x;
        eta_now[5] =  Mathf.Deg2Rad * transform.eulerAngles.y;

        // Compute rotation matrix
        UpdateRotationMatrix();

        // Gravity and buoyancy
        W = mass * gravity;
        B = -rho * gravity * Volume;

        G[0] = Jacv_1[0,2] * (W + B);
        G[1] = Jacv_1[1,2] * (W + B);
        G[2] = Jacv_1[2,2] * (W + B);
        G[3] = z_b * Jacv_1[1,2] * B;
        G[4] = -z_b * Jacv_1[0,2] * B;
        G[5] = 0.0f;
    }

    void Update()
    {
        dt = Time.deltaTime;

        // External disturbance in world frame
        World_dist[0] = X_dist_vel;
        World_dist[1] = Y_dist_vel;

        // Update orientation
        eta_now[3] = -Mathf.Deg2Rad * transform.eulerAngles.z;
        eta_now[4] = -Mathf.Deg2Rad * transform.eulerAngles.x;
        eta_now[5] =  Mathf.Deg2Rad * transform.eulerAngles.y;

        // Update rotation matrix
        UpdateRotationMatrix();

        // Convert world disturbance to body frame
        for (int i = 0; i < 3; i++) {
            dist_vel[i] = 0f;
            for (int j = 0; j < 3; j++) dist_vel[i] += Jacv_1[i, j] * World_dist[j];
        }

        // Hydrodynamic Coriolis terms
        C_nu[0] = -(M_A[2] + mass) * nu_now[5] * nu_now[1] + (M_A[1] + mass) * nu_now[4] * nu_now[2];
        C_nu[1] =  (M_A[2] + mass) * nu_now[5] * nu_now[0] - (M_A[0] + mass) * nu_now[3] * nu_now[2];
        C_nu[2] = -(M_A[1] + mass) * nu_now[4] * nu_now[0] + (M_A[0] + mass) * nu_now[3] * nu_now[1];
        C_nu[3] = -(M_A[5] + I_c[2] - M_A[4] - I_c[1]) * nu_now[4] * nu_now[5];
        C_nu[4] =  (M_A[5] + I_c[2] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[5];
        C_nu[5] = -(M_A[4] + I_c[1] - M_A[3] - I_c[0]) * nu_now[3] * nu_now[4];

        // Nonlinear drag (function of |velocity|)
        D_N[0] = 13.7f * Mathf.Abs(nu_now[0]);
        D_N[2] = 33.0f * Mathf.Abs(nu_now[2]);
        D_N[4] = 0.8f  * Mathf.Abs(nu_now[4]);

        // Recompute gravity vector (orientation may have changed)
        G[0] = Jacv_1[0,2] * (W + B);
        G[1] = Jacv_1[1,2] * (W + B);
        G[2] = Jacv_1[2,2] * (W + B);
        G[3] = z_b * Jacv_1[1,2] * B;
        G[4] = -z_b * Jacv_1[0,2] * B;

        // Dynamics integration
        for (int i = 0; i < 6; i++)
        {
            tau[i] = Thruster_tau.tau_output[i];
            D[i] = D_O[i] + D_N[i];

            nu_now_dot[i] = (tau[i] - (-C_nu[i] + D[i] * nu_now[i] + G[i])) / M[i];
            nu_now[i] += nu_now_dot[i] * dt + dist_vel[i];

            // Integration to position/orientation
            if (i == 0) pos_buf.z = nu_now[i] * dt;
            else if (i == 1) pos_buf.x = nu_now[i] * dt;
            else if (i == 2) pos_buf.y = -nu_now[i] * dt;
            else if (i == 3) angle_buf.z = -Mathf.Rad2Deg * nu_now[i] * dt;
            else if (i == 4) angle_buf.x = -Mathf.Rad2Deg * nu_now[i] * dt;
            else if (i == 5) angle_buf.y =  Mathf.Rad2Deg * nu_now[i] * dt;
        }

        transform.position += transform.rotation * pos_buf;
        transform.eulerAngles += angle_buf;
    }

    /// <summary>
    /// Computes the rotation matrix (Jacobian from body to world)
    /// </summary>
    private void UpdateRotationMatrix()
    {
        float phi = eta_now[3];
        float theta = eta_now[4];
        float psi = eta_now[5];

        Jacv_1[0,0] = Mathf.Cos(psi) * Mathf.Cos(theta);
        Jacv_1[0,1] = Mathf.Sin(psi) * Mathf.Cos(theta);
        Jacv_1[0,2] = -Mathf.Sin(theta);

        Jacv_1[1,0] = Mathf.Cos(psi) * Mathf.Sin(phi) * Mathf.Sin(theta) - Mathf.Cos(phi) * Mathf.Sin(psi);
        Jacv_1[1,1] = Mathf.Sin(psi) * Mathf.Sin(phi) * Mathf.Sin(theta) + Mathf.Cos(phi) * Mathf.Cos(psi);
        Jacv_1[1,2] = Mathf.Sin(phi) * Mathf.Cos(theta);

        Jacv_1[2,0] = Mathf.Cos(phi) * Mathf.Cos(psi) * Mathf.Sin(theta) + Mathf.Sin(phi) * Mathf.Sin(psi);
        Jacv_1[2,1] = Mathf.Cos(phi) * Mathf.Sin(psi) * Mathf.Sin(theta) - Mathf.Sin(phi) * Mathf.Cos(psi);
        Jacv_1[2,2] = Mathf.Cos(phi) * Mathf.Cos(theta);
    }
}
