# Unity-PBD-Script
simulate a line which consists of spheres and cylinders

##Script
* pbd01_oneSpring
* pbd02_twoSpring
* pbd03_fixedpointGravity
* pbd04_clothGravity
* pbd05_30x30cloth
* pbd06_collisionconstraint

## xpbd folder
含有 4 個範例
* cloth_sim
以 cube 繪製頂點,檢查頂點是否受重力向下,且constraint順利運作 (delta_frame_time, num_iters, num_substep, AlgorithmType)=(1/60, 10, 5, XPBD)
* cloth_TriangleMesh
以 mesh來繪製布料, 模擬速度提升許多
* cloth_hit_sphere
使用 collision constraint 分別針對固定即會移動的碰撞物
* aerodynamics
內含 5 種 condition (Vector3 wind_velocity, float drag_coeff, float lift_coeff) 
