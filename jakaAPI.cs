using System.Runtime.InteropServices;
using System.Text;
using jkType;

namespace jakaApi
{
	public delegate void CallBackFuncType(int error_code);
	public class jakaAPI
	{
        ///-----------------------------------------------------------------------------------------------------------///
        /// GENERAL ///
		
        /// <summary>
        /// Gets either IP of controller with specific name or list of all controllers in the network.
        /// </summary>
        /// <param name="controller_name">Name of controller - Leave empty to get IP's of all controllers in the network.</param>
        /// <param name="ip">Stringbuilder object to write the IP's into.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_controller_ip", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_controller_ip(char[] controller_name, StringBuilder ip);

        /// <summary>
        /// Gets either IP of controller with specific name or list of all controllers in the network.
        /// </summary>
        /// <param name="controller_name">Name of controller - Leave empty to get IP's of all controllers in the network.</param>
        /// <param name="ip">Stringbuilder object to write the IP's into.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_controller_ip", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_controller_ip([MarshalAs(UnmanagedType.LPStr)] string controller_name, StringBuilder ip);

        /// <summary>
        /// Creates cobot handler and binds it to pointer.
        /// </summary>
        /// <param name="ip">IP of the cobot to bind.</param>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "create_handler", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int create_handler(char[] ip, ref int handle);

        /// <summary>
        /// Creates cobot handler and binds it to pointer.
        /// </summary>
        /// <param name="ip">IP of the cobot to bind.</param>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "create_handler", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int create_handler([MarshalAs(UnmanagedType.LPStr)] string ip, ref int handle);

        /// <summary>
        /// Destroys cobot handler at pointer.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "destory_handler", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int destory_handler(ref int handle);

		/// <summary>
		/// Powers on the cobot. Will return after cobot is fully turned on (~2-5 seconds).
		/// </summary>
		/// <param name="handle">Pointer to cobot handler.</param>
		/// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
		[DllImport("jakaAPI.dll", EntryPoint = "power_on", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int power_on(ref int handle);
        /// <summary>
        /// Powers off the cobot, Will return after cobot is fully turned off.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "power_off", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int power_off(ref int handle);

        /// <summary>
        /// Shuts down the cabinet. WARNING: You will lose connection after calling this method, since the cabinet will shutdown completely.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "shut_down", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int shut_down(ref int handle);

        /// <summary>
        /// Unlocks the cobot brakes to allow it to move. Indicated color will switch to green when completed. Will wait for the cobot to unlock completely before returning.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "enable_robot", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int enable_robot(ref int handle);

        /// <summary>
        /// Locks the cobot brakes. Indicated color will switch to blue when completed. Will wait for cobot to fully lock before returning.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "disable_robot", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int disable_robot(ref int handle);





        ///-----------------------------------------------------------------------------------------------------------///
        /// MOVEMENT ///
        /// 
        /// -- JOG ----------------------------

        /// <summary>
        /// Jog movement. Move cobot in either cartesian space on a single axis or rotate specific joint in joint space.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="aj_num">
        ///		Select either Joint in joint mode or linear/rotational axis in cartesian space.<br/>
        ///		[0-5] Joint space: Axis [1-6], Cartesian space: [x, y, z, rx, ry, rz]<br/>
        /// </param>
        /// <param name="move_mode">Select between relative, absolute and continious move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/>
        ///		JKTYPE.MoveMode.CONTINUE // Continious move - Moves until stopped in direction given.<br/>
        /// </param>
        /// <param name="coord_type">Select either Joint space or Cartesian space:<br/>
        ///		JKTYPE.CoordType.COORD_BASE // Cartesian space - Base of cobot<br/>
        ///		JKTYPE.CoordType.COORD_JOINT // Joint space<br/>
        ///		JKTYPE.CoordType.COORD_TOOL // Cartesian space - Tool center point<br/>
        /// </param>
        /// <param name="vel_cmd">Velocity of the movement. Rad/s for joint movement, mm/s for cartesian movement. </param>
        /// <param name="pos_cmd">Distance to move. Rad for joint movement, mm for cartesian movement.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "jog", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jog(ref int handle, int aj_num, JKTYPE.MoveMode move_mode, JKTYPE.CoordType coord_type, double vel_cmd, double pos_cmd);

        /// <summary>
        /// Stops the jog movement at given axis.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="num">Select either Joint in joint mode or linear/rotational axis in cartesian space.<br/>
        ///		[0-5] Joint space: Axis [1-6], Cartesian space: [x, y, z, rx, ry, rz]<br/>
		///	</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "jog_stop", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int jog_stop(ref int handle, int num);
		
		/// -- JOINT MOVEMENT ----------------------------

        /// <summary>
        /// Joint move. 
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="joint_pos">JKTYPE.JointValue struct with joint values.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Joint movement speed in rad/s.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "joint_move", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int joint_move(ref int handle, ref JKTYPE.JointValue joint_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed);

        /// <summary>
        /// Joint move without slowind down at points. 
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="joint_pos">JKTYPE.JointValue struct with joint values.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Joint movement speed in rad/s.</param>
        /// <param name="acc">Joint movement acceleration in rad/s/s.</param>
        /// <param name="tol">End point tolerance of the movement in rad.</param>
        /// <param name="option_cond">Optional parameters for robot joints, if not needed, the value can be left unassigned, just fill in a null pointer.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "joint_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int joint_move_extend(ref int handle, ref JKTYPE.JointValue joint_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond);

        /// -- LINEAR MOVEMENT ----------------------------

        /// <summary>
        /// Linear move.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="end_pos">JKTYPE.CartesianPose struct with x, y, z, rx, ry, rz.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Cartesian movement speed in mm/s</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "linear_move", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int linear_move(ref int handle, ref JKTYPE.CartesianPose end_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed);

        /// <summary>
        /// Linear move without slowing down at points.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="end_pos">JKTYPE.CartesianPose struct with x, y, z, rx, ry, rz.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Cartesian movement speed in mm/s</param>
        /// <param name="acc">Cartesian movement acceleration in mm/s/s.</param>
        /// <param name="tol">End point tolerance of the movement in mm.</param>
        /// <param name="option_cond">Optional parameters for robot joints, if not needed, the value can be left unassigned, just fill in a null pointer.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "linear_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int linear_move_extend(ref int handle, ref JKTYPE.CartesianPose cart_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond);

        /// <summary>
        /// Linear move without slowing down at points..
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="end_pos">JKTYPE.CartesianPose struct with x, y, z, rx, ry, rz.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Cartesian movement speed in mm/s</param>
        /// <param name="acc">Cartesian movement acceleration in mm/s/s.</param>
        /// <param name="tol">End point tolerance of the movement in mm.</param>
        /// <param name="option_cond">Optional parameters for robot joints, if not needed, the value can be left unassigned, just fill in a null pointer.</param>
        /// <param name="ori_vel">Attitude velocity, unit rad/s.</param>
        /// <param name="ori_acc">Attitude acceleration, unit rad/s^2.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "linear_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int linear_move_extend_ori(ref int handle, ref JKTYPE.CartesianPose cart_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond, double ori_vel, double ori_acc);

        /// -- CIRCULAR MOVEMENT ----------------------------

        /// <summary>
        /// Circular move. Uses current position and two other positions to calculate circular path.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="end_pos">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for final position.</param>
        /// <param name="mid_pos">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for midway position.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Cartesian movement speed in mm/s</param>
        /// <param name="acc">Cartesian movement acceleration in mm/s/s.</param>
        /// <param name="tol">End point tolerance of the movement in mm.</param>
        /// <param name="option_cond">Optional parameters for robot joints, if not needed, the value can be left unassigned, just fill in a null pointer./param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "circular_move", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int circular_move(ref int handle, ref JKTYPE.CartesianPose end_pos, ref JKTYPE.CartesianPose mid_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond);

        /// <summary>
        /// Circular move without slowind down at points. Uses current position and two other positions to calculate circular path.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="end_pos">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for final position.</param>
        /// <param name="mid_pos">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for midway position.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Cartesian movement speed in mm/s</param>
        /// <param name="acc">Cartesian movement acceleration in mm/s/s.</param>
        /// <param name="tol">End point tolerance of the movement in mm.</param>
        /// <param name="option_cond">Optional parameters for robot joints, if not needed, the value can be left unassigned, just fill in a null pointer.</param>
        /// <param name="circle_cnt">Specifies the number of circles of the robot. A value of 0 is equivalent to circle_move.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "circular_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int circular_move_extend(ref int handle, ref JKTYPE.CartesianPose end_pos, ref JKTYPE.CartesianPose mid_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond, int circle_cnt);

        /// <summary>
        /// Circular move without slowind down at points. Uses current position and two other positions to calculate circular path.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="end_pos">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for final position.</param>
        /// <param name="mid_pos">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for midway position.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="is_block">Boolean - Wether the movement should block or not block other instructions.</param>
        /// <param name="speed">Cartesian movement speed in mm/s</param>
        /// <param name="acc">Cartesian movement acceleration in mm/s/s.</param>
        /// <param name="tol">End point tolerance of the movement in mm.</param>
        /// <param name="option_cond">Optional parameters for robot joints, if not needed, the value can be left unassigned, just fill in a null pointer.</param>
        /// <param name="circle_cnt">Specifies the number of circles of the robot. A value of 0 is equivalent to circle_move.</param>
        /// <param name="circle_mode">Specifies the mode of the robot's circular motion, the parameter explanation is as follows:
	    /// - 0: Fixed to use the axis angle of rotation angle less than 180° from the start attitude to the end attitude for attitude change; (current program)
	    /// - 1: Fixedly adopts the axis angle of the rotation angle from the start attitude to the termination attitude which is greater than 180° for attitude change;
	    /// - 2: Selection of whether the angle is less than 180° or more than 180° is automatically chosen according to the midpoint attitude;
	    /// - 3: The attitude pinch angle is always consistent with the arc axis. (Current whole circle motion)
        /// </param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "circular_move_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int circular_move_extend_mode(ref int handle, ref JKTYPE.CartesianPose end_pos, ref JKTYPE.CartesianPose mid_pos, JKTYPE.MoveMode move_mode, bool is_block, double speed, double acc, double tol, ref JKTYPE.OptionalCond option_cond, int circle_cnt, int circle_mode);

        /// -- SERVO MOVEMENT ----------------------------

        /// <summary>
        /// Enables the servo mode in order to use servo movement methods.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="enable">Boolean - Wether or not to enable servo mode.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_move_enable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int servo_move_enable(ref int handle, bool enable);

        /// <summary>
        /// Servo joint movement..
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="joint_pos">JKTYPE.JointValue struct with joint values.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_j", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int servo_j(ref int handle, ref JKTYPE.JointValue joint_pos, JKTYPE.MoveMode move_mode);

        /// <summary>
        /// Servo joint movement without slowing down at points.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="joint_pos">JKTYPE.JointValue struct with joint values.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="step_num">Distance the cobot should travel in rad in the direction of travel after reaching the point.<br/>
		///		The tickrate is 8ms. So the step_num should be a multiple of 8 when step_num >=1.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_j_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int servo_j_extend(ref int handle, ref JKTYPE.JointValue joint_pos, JKTYPE.MoveMode move_mode, int step_num);

        /// <summary>
        /// Servo cartesian movement.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="cartesian_pose">JKTYPE.CartesianPose struct with x, y, z, rx, ry, rz.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_p", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int servo_p(ref int handle, ref JKTYPE.CartesianPose cartesian_pose, JKTYPE.MoveMode move_mode);

        /// <summary>
        /// Servo cartesian movement without slowing down at points.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="cartesian_pose">JKTYPE.CartesianPose struct with x, y, z, rx, ry, rz.</param>
        /// <param name="move_mode">Select between relative and absolute move mode.<br/>
        ///		JKTYPE.MoveMode.ABS // Absolute move - Moves to exact position given.<br/>
        ///		JKTYPE.MoveMode.INCR // Relative move - Adds given value to current position.<br/></param>
        /// <param name="step_num">Distance the cobot should travel in mm in the direction of travel after reaching the point.<br/>
		///		The tickrate is 8ms. So the step_num should be a multiple of 8 when step_num >=1.<param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_p_extend", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int servo_p_extend(ref int handle, ref JKTYPE.CartesianPose cartesian_pose, JKTYPE.MoveMode move_mode, int step_num);

        /// <summary>
        /// Disable the filter for servo move commands.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_move_use_none_filter", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int servo_move_use_none_filter(ref int handle);

        /// <summary>
        /// Set 1st-order low-pass filter for servo move. It will take effect for both servo_j and servo_p commands.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="cutoffFreq">Cut-off frequency for low-pass filter.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_move_use_joint_LPF", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int servo_move_use_joint_LPF(ref int handle, double cutoffFreq);

        /// <summary>
        /// Set 3rd-order non-linear filter in joint space for servo move. It will take effect for both servo_j and 
	    /// servo_p commands.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="max_vr">Joint speed limit, in unit deg/s.</param>
        /// <param name="max_ar">Joint acceleration limit, in unit deg/s^2.</param>
        /// <param name="max_jr">Joint jerk limit, in unit deg/s^3.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_move_use_joint_NLF", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int servo_move_use_joint_NLF(ref int handle, double max_vr, double max_ar, double max_jr);

        /// <summary>
        /// Set 3rd-order non-linear filter in Cartesian space for servo move. It will only take effect for servo_p
	    /// since the filter will be applied to Cartesian position in the commands.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="max_vp">Speed limit, in unit mm/s.s</param>
        /// <param name="max_ap">Acceleration limit, in unit mm/s^2.</param>
        /// <param name="max_jp">Jerk limit, in unit mm/s^3.</param>
        /// <param name="max_vr">Orientation speed limit, in unit deg/s.</param>
        /// <param name="max_ar">Orientation acceleration limit, in unit deg/s^2.</param>
        /// <param name="max_jr">Orientation jerk limit, in unit deg/s^3.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_move_use_carte_NLF", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int servo_move_use_carte_NLF(ref int handle, double max_vp, double max_ap, double max_jp, double max_vr, double max_ar, double max_jr);

        /// <summary>
        /// Set multi-order mean filter in joint space for servo move. It will take effect for both servo_j and servo_p
	    /// commands.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="max_buf">Indicates the size of the mean filter buffer. If the filter buffer is set too small (less than 3), it
	    /// is likely to cause planning failure.The buffer value should not be too large(>100), which will bring computational
        /// burden to the controller and cause planning delay; as the buffer value increases, the planning delay time increases.
        /// </param>
        /// <param name="kp">Position filter coefficient.</param>
        /// <param name="kv">Velocity filter coefficient.</param>
        /// <param name="ka">Acceleration filter coefficient.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_move_use_joint_MMF", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int servo_move_use_joint_MMF(ref int handle, int max_buf, double kp, double kv, double ka);

        /// <summary>
        /// Set velocity look-ahead filter. It’s an extended version based on the multi-order filtering algorithm with 
	    /// look-ahead algorithm, which can be used for joints data and Cartesian data.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="max_buf">Buffer size of the mean filter. A larger buffer results in smoother results, but with higher precision
	    /// loss and longer planning lag time.
        /// </param>
        /// <param name="kp">Position filter coefficient. Reducing this coefficient will result in a smoother filtering effect, but a 
	    /// greater loss in position accuracy.Increasing this coefficient will result in a faster response and higher accuracy,
        /// but there may be problems with unstable operation/jitter, especially when the original data has a lot of noise.
        /// </param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "servo_speed_foresight", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int servo_speed_foresight(ref int handle, int max_buf, double kp);

        /// -- DRAG MODE -----------------------

        /// <summary>
        /// Enable drag mode.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="enable">Boolean - Wether or not to enable drag mode.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "drag_mode_enable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int drag_mode_enable(ref int handle, bool enable);

        /// <summary>
        /// Check if cobot is in drag mode.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="in_drag">Pointer to return boolean.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "is_in_drag_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int is_in_drag_mode(ref int handle, ref bool in_drag);

		/// -- GETTERS --------------------------------

        /// <summary>
        /// Get current TCP position.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="tcp_position">Pointer to JKTYPE.CartesianPose return variable. Will return current position in reference to chosen TCP configuration.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_tcp_position", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_tcp_position(ref int handle, ref JKTYPE.CartesianPose tcp_position);

        /// <summary>
        /// Get current joint position.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="joint_position">Pointer to JKTYPE.JointValue return variable.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_joint_position", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_joint_position(ref int handle, ref JKTYPE.JointValue joint_position);

        /// <summary>
        /// Abort current movement.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "motion_abort", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int motion_abort(ref int handle);


        ///-----------------------------------------------------------------------------------------------------------///
        /// IO ///

        /// <summary>
        /// Set digital output.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="type">Select IO type.
        ///		JKTYPE.IOType.IO_CABINET - Hardware IO connections inside the cabinet.</br>
        ///		JKTYPE.IOType.IO_TOOL - Hardware IO connections at the tool end.</br>
        ///		JKTYPE.IOType.IO_EXTEND - Extended IO</br>
        ///		JKTYPE.IOType.IO_REALY - Relay IO</br>
        ///		JKTYPE.IOType.IO_MODBUS_SLAVE - Virtual IO connections using Modbus.</br>
        ///		JKTYPE.IOType.IO_PROFINET_SLAVE - Virtual IO connections using Profinet.</br>
        ///		JKTYPE.IOType.IO_EIP_SLAVE - Virtual IO connections using EIP.</param>
        /// <param name="index">Index of DIO starting at 0.</param>
        /// <param name="value">Value to set the DIO.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_digital_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_digital_output(ref int handle, JKTYPE.IOType type, int index, bool value);

        /// <summary>
        /// Set analog output.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="type">Select IO type.
        ///		JKTYPE.IOType.IO_CABINET - Hardware IO connections inside the cabinet.</br>
        ///		JKTYPE.IOType.IO_TOOL - Hardware IO connections at the tool end.</br>
        ///		JKTYPE.IOType.IO_EXTEND - Extended IO</br>
        ///		JKTYPE.IOType.IO_REALY - Relay IO</br>
        ///		JKTYPE.IOType.IO_MODBUS_SLAVE - Virtual IO connections using Modbus.</br>
        ///		JKTYPE.IOType.IO_PROFINET_SLAVE - Virtual IO connections using Profinet.</br>
        ///		JKTYPE.IOType.IO_EIP_SLAVE - Virtual IO connections using EIP.</param>
        /// <param name="index">Index of AIO starting at 0.</param>
        /// <param name="value">Value to set the AIO.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_analog_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_analog_output(ref int handle, JKTYPE.IOType type, int index, float value);

        /// <summary>
        /// Get digital input.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="type">Select IO type.
        ///		JKTYPE.IOType.IO_CABINET - Hardware IO connections inside the cabinet.</br>
        ///		JKTYPE.IOType.IO_TOOL - Hardware IO connections at the tool end.</br>
        ///		JKTYPE.IOType.IO_EXTEND - Extended IO</br>
        ///		JKTYPE.IOType.IO_REALY - Relay IO</br>
        ///		JKTYPE.IOType.IO_MODBUS_SLAVE - Virtual IO connections using Modbus.</br>
        ///		JKTYPE.IOType.IO_PROFINET_SLAVE - Virtual IO connections using Profinet.</br>
        ///		JKTYPE.IOType.IO_EIP_SLAVE - Virtual IO connections using EIP.</param>
        /// <param name="index">Index of DIO starting at 0.</param>
        /// <param name="result">Pointer to result variable.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_digital_input", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_digital_input(ref int handle, JKTYPE.IOType type, int index, ref bool result);

        /// <summary>
        /// Get digital output.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="type">Select IO type.
        ///		JKTYPE.IOType.IO_CABINET - Hardware IO connections inside the cabinet.</br>
        ///		JKTYPE.IOType.IO_TOOL - Hardware IO connections at the tool end.</br>
        ///		JKTYPE.IOType.IO_EXTEND - Extended IO</br>
        ///		JKTYPE.IOType.IO_REALY - Relay IO</br>
        ///		JKTYPE.IOType.IO_MODBUS_SLAVE - Virtual IO connections using Modbus.</br>
        ///		JKTYPE.IOType.IO_PROFINET_SLAVE - Virtual IO connections using Profinet.</br>
        ///		JKTYPE.IOType.IO_EIP_SLAVE - Virtual IO connections using EIP.</param>
        /// <param name="index">Index of DIO starting at 0.</param>
        /// <param name="result">Pointer to result variable.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_digital_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_digital_output(ref int handle, JKTYPE.IOType type, int index, ref bool result);

        /// <summary>
        /// Get analog input.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="type">Select IO type.
        ///		JKTYPE.IOType.IO_CABINET - Hardware IO connections inside the cabinet.</br>
        ///		JKTYPE.IOType.IO_TOOL - Hardware IO connections at the tool end.</br>
        ///		JKTYPE.IOType.IO_EXTEND - Extended IO</br>
        ///		JKTYPE.IOType.IO_REALY - Relay IO</br>
        ///		JKTYPE.IOType.IO_MODBUS_SLAVE - Virtual IO connections using Modbus.</br>
        ///		JKTYPE.IOType.IO_PROFINET_SLAVE - Virtual IO connections using Profinet.</br>
        ///		JKTYPE.IOType.IO_EIP_SLAVE - Virtual IO connections using EIP.</param>
        /// <param name="index">Index of AIO starting at 0.</param>
        /// <param name="result">Pointer to result variable.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_analog_input", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_analog_input(ref int handle, JKTYPE.IOType type, int index, ref float result);

        /// <summary>
        /// Get analog output.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="type">Select IO type.
        ///		JKTYPE.IOType.IO_CABINET - Hardware IO connections inside the cabinet.</br>
        ///		JKTYPE.IOType.IO_TOOL - Hardware IO connections at the tool end.</br>
        ///		JKTYPE.IOType.IO_EXTEND - Extended IO</br>
        ///		JKTYPE.IOType.IO_REALY - Relay IO</br>
        ///		JKTYPE.IOType.IO_MODBUS_SLAVE - Virtual IO connections using Modbus.</br>
        ///		JKTYPE.IOType.IO_PROFINET_SLAVE - Virtual IO connections using Profinet.</br>
        ///		JKTYPE.IOType.IO_EIP_SLAVE - Virtual IO connections using EIP.</param>
        /// <param name="index">Index of AIO starting at 0.</param>
        /// <param name="result">Pointer to result variable.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_analog_output", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_analog_output(ref int handle, JKTYPE.IOType type, int index, ref float result);

        /// <summary>
        /// Check if extension IO is running.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <param name="is_running">Pointer to result boolean.</param>
        /// <returns>Returns err_code as int. Refer to documentation for err_codes.</returns>
        [DllImport("jakaAPI.dll", EntryPoint = "is_extio_running", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int is_extio_running(ref int handle, ref bool is_running);


        ///-----------------------------------------------------------------------------------------------------------///
        /// PROGRAM MANAGEMENT ///
		
        /// <summary>
        /// Run the current program.
        /// </summary>
        /// <param name="handle">Pointer to cobot handler.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "program_run", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int program_run(ref int handle);
		
		/// <summary>
		/// Stop current running program.
		/// </summary>
		/// <param name="handle">Pointer to cobot handler.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "program_pause", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int program_pause(ref int handle);
		
		/// <summary>
		/// Resume current paused program.
		/// </summary>
		/// <param name="handle">Pointer to cobot handler.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "program_resume", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int program_resume(ref int handle);
		
		/// <summary>
		/// Abort current program.
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "program_abort", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int program_abort(ref int handle);

        /// <summary>
        /// Load program as current program.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="file">File path to JAKA program zip file. Contains jks, zu, vi files.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "program_load", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int program_load(ref int handle, char[] file);

        /// <summary>
        /// Load program as current program.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="file">File path to JAKA program zip file. Contains jks, zu, vi files.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "program_load", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int program_load(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string file);

		/// <summary>
		/// Get file path to loaded program.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="file">Strinbuilder object to create file path.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_loaded_program", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_loaded_program(ref int handle, StringBuilder file);

		/// <summary>
		/// Get current line number in running program.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="curr_line">Pointer to return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_current_line", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_current_line(ref int handle, ref int curr_line);

		/// <summary>
		/// Get current program state.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="status">Pointer to JKTYPE.ProgramState return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_program_state", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_program_state(ref int handle, ref JKTYPE.ProgramState status);


        ///-----------------------------------------------------------------------------------------------------------///
        /// TOOL CENTER POINT ///

        /// <summary>
        /// Save TCP with corresponding name in one of the TCP save locations.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id">ID of wanted save location [1-10].</param>
        /// <param name="tcp">JKTYPE.CartesianPose with x, y, z, rx, ry, rz as the TCP offset.</param>
        /// <param name="name">Name of the TCP.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_tool_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_tool_data(ref int handle, int id, ref JKTYPE.CartesianPose tcp, char[] name);

        /// <summary>
        /// Save TCP with corresponding name in one of the TCP save locations.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id">ID of wanted save location [1-10].</param>
        /// <param name="tcp">JKTYPE.CartesianPose with x, y, z, rx, ry, rz as the TCP offset.</param>
        /// <param name="name">Name of the TCP.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_tool_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_tool_data(ref int handle, int id, ref JKTYPE.CartesianPose tcp, [MarshalAs(UnmanagedType.LPStr)] string name);

		/// <summary>
		/// Get TCP information at given id.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="id">TCP id. [1-10]</param>
		/// <param name="tcp">Pointer to JKTYPE.CartesianPose return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_tool_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_tool_data(ref int handle, int id, ref JKTYPE.CartesianPose tcp);

        /// <summary>
        /// Set current TCP to use.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id">TCP id. [0-10] (0 is tool flange center)</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_tool_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_tool_id(ref int handle, int id);
		
		/// <summary>
		/// Get id of current TCP in use.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="id">Pointer to return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_tool_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_tool_id(ref int handle, ref int id);


        ///-----------------------------------------------------------------------------------------------------------///
        /// USER COORDINATE FRAME ///

        /// <summary>
        /// Save user coordinate frame in one of the save locations.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id">ID of wanted save location [1-10].</param>
        /// <param name="user_frame">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for the user frame offset.</param>
        /// <param name="name">Name of the user frame.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_user_frame_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_user_frame_data(ref int handle, int id, ref JKTYPE.CartesianPose user_frame, char[] name);

        /// <summary>
        /// Save user coordinate frame in one of the save locations.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id">ID of wanted save location [1-10].</param>
        /// <param name="user_frame">JKTYPE.CartesianPose with x, y, z, rx, ry, rz for the user frame offset.</param>
        /// <param name="name">Name of the user frame.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_user_frame_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_user_frame_data(ref int handle, int id, ref JKTYPE.CartesianPose user_frame, [MarshalAs(UnmanagedType.LPStr)] string name);

		/// <summary>
		/// Get user coordinate frame information at given id.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="id">User coordinate frame id. [1-10]</param>
		/// <param name="user_frame"></param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_user_frame_data", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_user_frame_data(ref int handle, int id, ref JKTYPE.CartesianPose user_frame);

        /// <summary>
        /// Set current user coordinate frame to use.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id">User coordinate frame id. [0-10] (0 is world coordinates).</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_user_frame_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_user_frame_id(ref int handle, int id);

        /// <summary>
        /// Get id of current user coordinate frame in use.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="id">Pointer to return variable.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_user_frame_id", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_user_frame_id(ref int handle, ref int id);


        ///-----------------------------------------------------------------------------------------------------------///
        /// ROBOT STATUS ///

        /// <summary>
        /// Get current cobot state.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="state">Pointer to JKTYPE.RobotState return variable. Check documentation or JKType file for state explanation.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_robot_state", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_robot_state(ref int handle, ref JKTYPE.RobotState state);
		
		/// <summary>
		/// Check if cobot has collided.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="in_collision">Pointer to return boolean.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "is_in_collision", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int is_in_collision(ref int handle, ref bool in_collision);

        /// <summary>
        /// Check if the cobot is now on soft limit.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="on_limit">Pointer for the returned result.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "is_on_limit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int is_on_limit(ref int handle, ref bool on_limit);
		
		/// <summary>
		/// Check if cobot has arrived at its position yet.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="in_pos">Pointer to return boolean.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "is_in_pos", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int is_in_pos(ref int handle, ref bool in_pos);
		
		/// <summary>
		/// Get the current threshold that the cobot allows for the is_in_pos method to return true.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="thresholding">Pointer to return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_in_pos_thresholding", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_in_pos_thresholding(ref int handle, ref double thresholding);
		
		/// <summary>
		/// Set the threshold that the cobot allows for the is_in_pos method to return true.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="thresholding">Threshold in mm.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "set_in_pos_thresholding", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_in_pos_thresholding(ref int handle, double thresholding);

		/// <summary>
		/// Reset collision.
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "collision_recover", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int collision_recover(ref int handle);

        /// <summary>
        /// Set collision level.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="level">Collision level [0-5]</br>
        ///		0: Close collision.</br>
        ///		1: 25N Collision threshold.</br>
		///		2: 50N Collision threshold.</br>
		///		3: 75N Collision threshold.</br>
		///		4: 100N Collision threshold.</br>
		///		5: 125N Collision threshold.</br>
        /// </param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_collision_level", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_collision_level(ref int handle, int level);
		
		/// <summary>
		/// Get current collision level.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="level">Pointer to return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_collision_level", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_collision_level(ref int handle, ref int level);


        ///-----------------------------------------------------------------------------------------------------------///
        /// JKTYPE HELPER FUNCTIONS ///

        /// <summary>
        /// Calculate kine inverse of given pose under current tool, installation angle and user coordinate frame.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ref_pos">JKTYPE.JointValue reference joint position. Recommended to use current position.</param>
        /// <param name="cartesian_pose">JKTYPE.CartesianPose given position to convert to joint space.</param>
        /// <param name="joint_pos">Pointer to JKTYPE.JointValue return variable.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "kine_inverse", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int kine_inverse(ref int handle, ref JKTYPE.JointValue ref_pos, ref JKTYPE.CartesianPose cartesian_pose, ref JKTYPE.JointValue joint_pos);
		
		/// <summary>
		/// Calculate kine forward of given pose.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="joint_pos">JKTYPE.Joint value given position to convert to cartesian space.</param>
		/// <param name="cartesian_pose">Pointer to JKTYPE.CartesianPose return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "kine_forward", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int kine_forward(ref int handle, ref JKTYPE.JointValue joint_pos, ref JKTYPE.CartesianPose cartesian_pose);
		
		/// <summary>
		/// Converts rpy struct to rotational matrix struct.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="rpy">JKTYPE.Rpy to convert to rotational matrix.</param>
		/// <param name="rot_matrix">Pointer to JKTYPE.RotMatrix return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "rpy_to_rot_matrix", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rpy_to_rot_matrix(ref int handle, ref JKTYPE.Rpy rpy, ref JKTYPE.RotMatrix rot_matrix);
		
		/// <summary>
		/// Converts rotational matrix struct to rpy struct.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="rot_matrix">JKTYPE.RotMatrix to convert to rpy.</param>
		/// <param name="rpy">Pointer to JKTYPE.Rpy return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "rot_matrix_to_rpy", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rot_matrix_to_rpy(ref int handle, ref JKTYPE.RotMatrix rot_matrix, ref JKTYPE.Rpy rpy);
		
		/// <summary>
		/// Converts quaternion struct to rotational matrix struct.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="quaternion">JKTYPE.Quaternion to convert to rotational matrix.</param>
		/// <param name="rot_matrix">Pointer to JKTYPE.RotMatrix return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "quaternion_to_rot_matrix", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int quaternion_to_rot_matrix(ref int handle, ref JKTYPE.Quaternion quaternion, ref JKTYPE.RotMatrix rot_matrix);
		
		/// <summary>
		/// Converts rotational matrix struct to quaternion struct.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="rot_matrix">JKTYPE.RotMatrix to convert to quaternion.</param>
		/// <param name="quaternion">Pointer to JKTYPE.Quaternion return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "rot_matrix_to_quaternion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rot_matrix_to_quaternion(ref int handle, ref JKTYPE.RotMatrix rot_matrix, ref JKTYPE.Quaternion quaternion);
		
		/// <summary>
		/// Enables torque control.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="enable">Boolean - Wether or not to enable torque Control.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "torque_control_enable", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int torque_control_enable(ref int handle, bool enable);

        // [DllImport("jakaAPI.dll", EntryPoint = "torque_feedforward", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        // public static extern int torque_feedforward(ref int handle, JKTYPE.TorqueValue tor_val, int grv_flag);

        ///-----------------------------------------------------------------------------------------------------------///
        /// PAYLOAD ///

        /// <summary>
        /// Set Payload of the attached accessories.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="payload">JKType.PayLoad consists of weight of the attachements and center of gravity offset.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_payload", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_payload(ref int handle, ref JKTYPE.PayLoad payload);
		
		/// <summary>
		/// Get current set payload.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="payload">Pointer to JKTYPE.PayLoad return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_payload", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_payload(ref int handle, ref JKTYPE.PayLoad payload);


        ///-----------------------------------------------------------------------------------------------------------///
        /// MISC ///
		
        /// <summary>
        /// Set callback function for debugging cobot errors.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="func">CallBackFunType for error handling.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_error_handler", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_error_handler(ref int handle, CallBackFuncType func);
		
		/// <summary>
		/// Get current sdk version
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="version">StringBuilder version for returning sdk version.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_sdk_version", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_sdk_version(ref int handle, StringBuilder version);

        /// <summary>
        /// Get sdk file path.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filepath">Pointer to file path return variable.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_SDK_filepath", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_SDK_filepath(ref int handle, ref char[] filepath);

        /// <summary>
        /// Get sdk file path.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filepath">Pointer to file path return variable.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_SDK_filepath", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_SDK_filepath(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string filepath);

        /// <summary>
		/// TODO no pointer.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="size"></param>
		/// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_SDK_filepath", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_SDK_filepath(ref char[] path, int size);

        /// <summary>
		/// TODO no Pointer.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="size"></param>
		/// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_SDK_filepath", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_SDK_filepath(ref StringBuilder path, int size);
        /// <summary>
        /// Get current robot status.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="status">Pointer to JKTYPE.RobotStatus return variable.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_robot_status", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_robot_status(ref int handle, ref JKTYPE.RobotStatus status);
		
		/// <summary>
		/// Set errorcode file path for debugging.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="path">Stringbuilder path to set path for log files.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "set_errorcode_file_path", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_errorcode_file_path(ref int handle, StringBuilder path);
		
		/// <summary>
		/// Get last error.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="code">Pointer to JKTYPE.ErrorCode return variable.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_last_error", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_last_error(ref int handle, ref JKTYPE.ErrorCode code);

        /// <summary>
        /// Clear the last given error.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "clear_error", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int clear_error(ref int handle);

        /// <summary>
        /// Enable or disable debug mode.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mode">Boolean - Wether or not to enable debug mode.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_debug_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_debug_mode(ref int handle, bool mode);


        ///-----------------------------------------------------------------------------------------------------------///
        /// TRAJECTORY ///
		
        /// <summary>
        /// Sets the trajectory recording parameters.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="para">Trajectory recording parameters.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_traj_config", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_traj_config(ref int handle, ref JKTYPE.TrajTrackPara para);
		
		/// <summary>
		/// Gets the trajectory recording parameters.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="para">Reference to a TrajTrackPara object which will have the trajectory recording parameters saved into.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_traj_config", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_traj_config(ref int handle, ref JKTYPE.TrajTrackPara para);

        /// <summary>
        /// Acquisition track reproduction data control switch.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mode">Trajectory recording (sampling) mode. TRUE to start trajectory recording (sampling) and FALSE to disable.</param>
        /// <param name="filename">File name to save the trajectory recording results</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_traj_sample_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_traj_sample_mode(ref int handle, bool mode, char[] filename);

        /// <summary>
        /// Acquisition track reproduction data control switch.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mode">Trajectory recording (sampling) mode. TRUE to start trajectory recording (sampling) and FALSE to disable.</param>
        /// <param name="filename">File name to save the trajectory recording results</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_traj_sample_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_traj_sample_mode(ref int handle, bool mode, [MarshalAs(UnmanagedType.LPStr)] string filename);

        /// <summary>
        /// Get current trajectory recording status of the cobot.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sample_statuse">Pointer for the returned status.TRUE if it's now recording, FALSE if the data recording is over or not recording.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_traj_sample_status", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_traj_sample_status(ref int handle, ref bool sample_statuse);

        /// <summary>
        /// Get all the existing trajectory recordings of the cobot.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_exist_traj_file_name", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_exist_traj_file_name(ref int handle, ref JKTYPE.MultStrStorType filename);

        /// <summary>
        /// Rename the specified trajectory recording file.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="src">Source file name of the trajectory recording.</param>
        /// <param name="dest">Destination file name of the trajectory recording, the length must be no more than 100 characters.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "rename_traj_file_name", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rename_traj_file_name(ref int handle, ref char[] src, ref char[] dest);

        /// <summary>
        /// Rename the specified trajectory recording file.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="src">Source file name of the trajectory recording.</param>
        /// <param name="dest">Destination file name of the trajectory recording, the length must be no more than 100 characters.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "rename_traj_file_name", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rename_traj_file_name(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string src, [MarshalAs(UnmanagedType.LPStr)] string dest);

        /// <summary>
        /// Delete the specified trajectory file.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename">File name of the trajectory recording.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "remove_traj_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int remove_traj_file(ref int handle, ref char[] filename);

        /// <summary>
        /// Delete the specified trajectory file.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename">File name of the trajectory recording.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "remove_traj_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int remove_traj_file(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string filename);

        /// <summary>
        /// Generate program scripts from specified trajectory recording file.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename">Trajectory recording file.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "generate_traj_exe_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int generate_traj_exe_file(ref int handle, char[] filename);

        /// <summary>
        /// Generate program scripts from specified trajectory recording file.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename">Trajectory recording file.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "generate_traj_exe_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int generate_traj_exe_file(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string filename);


        ///-----------------------------------------------------------------------------------------------------------///
        /// SENSOR ///

        /// <summary>
        /// Set the type/brand of torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sensor_brand">Type/brand of the torque sensor.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_torsenosr_brand", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_torsenosr_brand(ref int handle, int sensor_brand);

        /// <summary>
        /// Get the type/brand of torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sensor_brand">Type/brand of the torque sensor.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_torsenosr_brand", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_torsenosr_brand(ref int handle, ref int sensor_brand);
		
		/// <summary>
		/// Enable or disable force torque sensor.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="sensor_mode">1: on, 2: off.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "set_torque_sensor_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_torque_sensor_mode(ref int handle, int sensor_mode);

        /// <summary>
        /// Start to identify payload of the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="joint_pos">End joint position of the trajectory.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "start_torq_sensor_payload_identify", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int start_torq_sensor_payload_identify(ref int handle, ref JKTYPE.JointValue joint_pos);

        /// <summary>
        /// Get identified payload of the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="handledentify_status">Pointer to the returned identified payload.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_torq_sensor_identify_staus", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_torq_sensor_identify_staus(ref int handle, ref int handledentify_status);

        /// <summary>
        /// Get identified payload of the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="payload">payload Pointer to the returned identified payload.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_torq_sensor_payload_identify_result", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_torq_sensor_payload_identify_result(ref int handle, ref JKTYPE.PayLoad payload);

        /// <summary>
        /// Set the payload for the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="payload">Payload of torque sensor.	</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_torq_sensor_tool_payload", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_torq_sensor_tool_payload(ref int handle, ref JKTYPE.PayLoad payload);

        /// <summary>
        /// Get current payload of the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="payload">payload Pointer to the returned payload.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_torq_sensor_tool_payload", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_torq_sensor_tool_payload(ref int handle, ref JKTYPE.PayLoad payload);

        /// <summary>
        /// Setup the communication for the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="type">Commnunication type of the torque sensor, 0: TCP/IP, 1: RS485.</param>
        /// <param name="ip_addr">IP address of the torque sensor. Only for TCP/IP.</param>
        /// <param name="port">Port for the torque sensor. only for TCP/IP</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_torque_sensor_comm", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_torque_sensor_comm(ref int handle, int type, ref char[] ip_addr, int port);

        /// <summary>
        /// Setup the communication for the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="type">Commnunication type of the torque sensor, 0: TCP/IP, 1: RS485.</param>
        /// <param name="ip_addr">IP address of the torque sensor. Only for TCP/IP.</param>
        /// <param name="port">Port for the torque sensor. only for TCP/IP</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_torque_sensor_comm", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_torque_sensor_comm(ref int handle, int type, [MarshalAs(UnmanagedType.LPStr)] string ip_addr, int port);

        /// <summary>
        /// Get the communication settings of the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="type">Pointer for the returned commnunication type.</param>
        /// <param name="ip_addr">Pointer for the returned IP address.</param>
        /// <param name="port">Pointer for the returned port.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_torque_sensor_comm", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_torque_sensor_comm(ref int handle, ref int type, [MarshalAs(UnmanagedType.LPStr)] StringBuilder ip_addr, ref int port);

		/// <summary>
		/// Disable force control.
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "disable_force_control", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int disable_force_control(ref int handle);

        /// <summary>
        /// Set the torque sensor low-pass filter parameter for force control.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="torque_sensor_filter">Filter parameter, unit：Hz</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_torque_sensor_filter", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_torque_sensor_filter(ref int handle, float torque_sensor_filter);

        /// <summary>
        /// Get the filter parameter of force control.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="torque_sensor_filter">Pointer for the returned filter parameter, unit：Hz</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_torque_sensor_filter", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_torque_sensor_filter(ref int handle, ref float torque_sensor_filter);

        /// <summary>
        /// Set soft force or torque limit for the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="torque_sensor_soft_limit">Soft limit, fx/fy/fz is force limit in unit N and tx/ty/tz is torque limit unit：N*m.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_torque_sensor_soft_limit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_torque_sensor_soft_limit(ref int handle, JKTYPE.FTxyz torque_sensor_soft_limit);

        /// <summary>
        /// Get current soft force or torque limit of the torque sensor.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="torque_sensor_soft_limit">Pointer for the returned soft limits.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_torque_sensor_soft_limit", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_torque_sensor_soft_limit(ref int handle, ref JKTYPE.FTxyz torque_sensor_soft_limit);


        ///-----------------------------------------------------------------------------------------------------------///
        /// FT CTRL ///

        /// <summary>
        /// Set the coordinate or frame for the force control.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ftFrame">Coordinate or frame option. 0: tool frame, 1:world frame.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_ft_ctrl_frame", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_ft_ctrl_frame(ref int handle, int ftFrame);

        /// <summary>
        /// Get the coordinate or frame of the force control.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ftFrame">Pointer for the returned frame option.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_ft_ctrl_frame", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_ft_ctrl_frame(ref int handle,ref int ftFrame);


        ///-----------------------------------------------------------------------------------------------------------///
        /// TIO ///

        /// <summary>
        /// Set voltage parameter for TIO of the cobot. It only takes effect for TIO with hardware version 3.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="vout_enable">Option to enable voltage output. 0:turn off， 1:turn on.</param>
        /// <param name="vout_vol">Option to set output voltage. 0: 24V, 1:12V.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_tio_vout_param", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_tio_vout_param(ref int handle, int vout_enable,int vout_vol);

        /// <summary>
        /// Get voltage parameter of TIO of the cobot. It only takes effect for TIO with hardware version 3.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="vout_enable">Pointer for the returned voltage enabling option.</param>
        /// <param name="vout_vol">Pointer for the returned output voltage option.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_tio_vout_param", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_tio_vout_param(ref int handle,ref int vout_enable,ref int vout_vol);

        /// <summary>
        /// Set timeout of motion block
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="seconds">Timeout, unit: second</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_block_wait_timeout", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_block_wait_timeout(ref int handle, float seconds);

        /// <summary>
        /// Add or modify the signal for TIO RS485 channels.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sign_info">Definition data of the signal.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "add_tio_rs_signal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int add_tio_rs_signal(ref int handle, JKTYPE.SignInfo sign_info);

        /// <summary>
        /// Delete the specified signal for TIO RS485 channel.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sig_name">Signal name.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "del_tio_rs_signal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int del_tio_rs_signal(ref int handle, char[] sig_name);

        /// <summary>
        /// Delete the specified signal for TIO RS485 channel.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sig_name">Signal name.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "del_tio_rs_signal", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int del_tio_rs_signal(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string sig_name);

        /// <summary>
        /// Send a command to the specified RS485 channel.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="chn_id">ID of the RS485 channel in TIO.</param>
        /// <param name="data">Command data.</param>
        /// <param name="buffsize">Buffer size.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "send_tio_rs_command", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int send_tio_rs_command(ref int handle, int chn_id, byte[] data, int buffsize);

        /// <summary>
        /// Get all the defined signals in TIO module.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sign_info">Pointer for the returned signal list.</param>
        /// <param name="size">Pointer for the size of the returned signal list.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_rs485_signal_info", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_rs485_signal_info(ref int handle, [In, Out] JKTYPE.SignInfo[] sign_info, ref int size);

        /// <summary>
        /// Set tio mode.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pin_type">Tio type 0 for DI Pins, 1 for DO Pins, 2 for AI Pins</param>
        /// <param name="pin_mode">T mode DI Pins: 0:0x00 DI2 is NPN,DI1 is NPN,1:0x01 DI2 is NPN,DI1 is PNP, 2:0x10 DI2 is PNP,DI1 is NPN,3:0x11 DI2 is PNP,DI1 is PNP
        /// DO Pins: Low 8-bit data high 4-bit for DO2 configuration, low 4-bit for DO1 configuration, 0x0 DO for NPN output, 0x1 DO for PNP output, 0x2 DO for push-pull output, 0xF RS485H interface
        /// AI Pins: 0: analog input function enable, RS485L disable, 1: RS485L interface enable, analog input function disable
        /// </param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_tio_pin_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_tio_pin_mode(ref int handle, int pin_type, int pin_mode);

        /// <summary>
        /// Get mode of the TIO pin of specified type.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="pin_type">Tio type 0 for DI Pins, 1 for DO Pins, 2 for AI Pins</param>
        /// <param name="pin_mode">tio mode DI Pins: 0:0x00 DI2 is NPN,DI1 is NPN,1:0x01 DI2 is NPN,DI1 is PNP, 2:0x10 DI2 is PNP,DI1 is NPN,3:0x11 DI2 is PNP,DI1 is PNP
        ///DO Pins: Low 8-bit data high 4-bit for DO2 configuration, low 4-bit for DO1 configuration, 0x0 DO for NPN output, 0x1 DO for PNP output, 0x2 DO for push-pull output, 0xF RS485H interface
        ///AI Pins: 0: analog input function enable, RS485L disable, 1: RS485L interface enable, analog input function disable
        ///</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_tio_pin_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_tio_pin_mode(ref int handle, int pin_type, ref int pin_mode);

        /// <summary>
        /// Setup communication for specified RS485 channel.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mod_rtu_com">ModRtuComm When the channel mode is set to Modbus RTU, you need to specify the Modbus slave node ID additionally.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_rs485_chn_comm", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_rs485_chn_comm(ref int handle, JKTYPE.ModRtuComm mod_rtu_com);

        /// <summary>
        /// Get RS485 commnunication setting.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mod_rtu_com">Pointer for the returned communication settings.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_rs485_chn_comm", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_rs485_chn_comm(ref int handle, ref JKTYPE.ModRtuComm mod_rtu_com);

        /// <summary>
        /// Set the mode for specified RS485 channel.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="chn_id">Channel id. 0 for RS485H, channel 1; 1 for RS485L, channel 2.</param>
        /// <param name="chn_mode">Mode to indicate the usage of RS485 channel. 0 for Modbus RTU, 1 for Raw RS485, 2 for torque sensor.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_rs485_chn_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int set_rs485_chn_mode(ref int handle, int chn_id, int chn_mode);

        /// <summary>
        /// Get the mode of specified RS485 channel.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="chn_id">Channel id. 0: RS485H, channel 1; 1: RS485L, channel 2</param>
        /// <param name="chn_mode">Pointer for the returned mode. 0: Modbus RTU, 1: Raw RS485, 2, torque sensor.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_rs485_chn_mode", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_rs485_chn_mode(ref int handle, int chn_id, ref int chn_mode);


        ///-----------------------------------------------------------------------------------------------------------///
        /// FTP ///

        /// <summary>
        /// Establish ftp connection with controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "init_ftp_client", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int init_ftp_client(ref int handle);

        /// <summary>
        /// Disconnect ftp from controller
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "close_ftp_client", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int close_ftp_client(ref int handle);

        /// <summary>
        /// Download a file of the specified type and name from the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="local">The absolute path to the file name to be downloaded locally.</param>
        /// <param name="remote">The absolute path to the file name inside the controller.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "download_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int download_file(ref int handle, char[] local, char[] remote, int opt);

        /// <summary>
        /// Download a file of the specified type and name from the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="local">The absolute path to the file name to be downloaded locally.</param>
        /// <param name="remote">The absolute path to the file name inside the controller.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "download_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int download_file(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string local, [MarshalAs(UnmanagedType.LPStr)] string remote, int opt);

        /// <summary>
        /// Upload a file of a specified type and name to the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="local">Absolute path to local file name.</param>
        /// <param name="remote">Absolute path of the file name to be uploaded inside the controller.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "upload_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int upload_file(ref int handle, char[] local, char[] remote, int opt);

        /// <summary>
        /// Upload a file of a specified type and name to the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="local">Absolute path to local file name.</param>
        /// <param name="remote">Absolute path of the file name to be uploaded inside the controller.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "upload_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int upload_file(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string local, [MarshalAs(UnmanagedType.LPStr)] string remote, int opt);

        /// <summary>
        /// Delete a file of the specified type and name from the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="remote">Controller internal file name.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "del_ftp_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int del_ftp_file(ref int handle, char[] remote, int opt);

        /// <summary>
        /// Delete a file of the specified type and name from the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="remote">Controller internal file name.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "del_ftp_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int del_ftp_file(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string remote, int opt);

        /// <summary>
        /// Rename a file of the type and name specified by the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="remote">Original name of the controller's internal file name.</param>
        /// <param name="des">The target name to rename.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "rename_ftp_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rename_ftp_file(ref int handle, char[] remote, char[] des, int opt);

        /// <summary>
        /// Rename a file of the type and name specified by the controller.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="remote">Original name of the controller's internal file name.</param>
        /// <param name="des">The target name to rename.</param>
        /// <param name="opt">Option: 1 single file 2 folder.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "rename_ftp_file", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int rename_ftp_file(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string remote, [MarshalAs(UnmanagedType.LPStr)] string des, int opt);

        /// <summary>
        /// No Pointer
        /// Get the directory of the FTP service. 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="remote">Pointer for the returned FIP directory. like "/track/" or "/program/"</param>
        /// <param name="type">Type of the file. 0: file and folder, 1: single file, 2: folder</param>
        /// <param name="ret">Returned structure of the directory in string format.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_ftp_dir", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_ftp_dir(ref int handle, char[] remote, int type, StringBuilder ret );

        /// <summary>
        /// No Pointer
        /// Get the directory of the FTP service. 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="remote">Pointer for the returned FIP directory. like "/track/" or "/program/"</param>
        /// <param name="type">Type of the file. 0: file and folder, 1: single file, 2: folder</param>
        /// <param name="ret">Returned structure of the directory in string format.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_ftp_dir", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		public static extern int get_ftp_dir(ref int handle, [MarshalAs(UnmanagedType.LPStr)] string remote, int type, StringBuilder ret );

        ///-----------------------------------------------------------------------------------------------------------///

        /// <summary>
        /// Setting the robot run rate.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rapid_rate">Value of the velociry rate, range from [0,1].</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_rapidrate", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_rapidrate(ref int handle, double rapid_rate);

        /// <summary>
        /// Get the robot runtime rate.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="rapid_rate">Pointer for the returned current velocity rate.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_rapidrate", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_rapidrate(ref int handle, ref double rapid_rate);

        /// <summary>
        /// Set parameters for admittance control of the cobot.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="axis">ID of the axis to be controlled, axis with ID 0 to 5 corresponds to x, y, z, Rx, Ry, Rz.</param>
        /// <param name="opt">Enable flag. 0: disable, 1: enable.</param>
        /// <param name="ftUser">Force to move the cobot in maximum speed.</param>
        /// <param name="ftConstant">Set to 0 when operate manually.</param>
        /// <param name="ftNnormalTrack">Set to 0 when operate manually.</param>
        /// <param name="ftReboundFK">Ability to go back to initial position.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_admit_ctrl_config", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_admit_ctrl_config(ref int handle, int axis, int opt, double ftUser, double ftConstant, int ftNnormalTrack, double ftReboundFK);

        /// <summary>
        /// Enable or disable the admittance control of the cobot. It will only work when a torque sensor is equiped.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="enable_flag">Option to indicate enable or disable the admittance control. 1 to enable and 0 to disable
	    /// the admittance control.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "enable_admittance_ctrl", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int enable_admittance_ctrl(ref int handle, int enable_flag);

        /// <summary>
        /// Set compliance control type and sensor initialization status.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sensor_compensation">Whether to enable sensor compensation, 1 means enable is initialized, 0 means not initialized.</param>
        /// <param name="compliance_type">0 for not using any kind of compliance control method 1 for constant force compliance control, 2 for speed compliance control</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_compliant_type", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_compliant_type(ref int handle, int sensor_compensation, int compliance_type);

        /// <summary>
        /// Get compliance control type and sensor initialization status.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="sensor_compensation">Whether to enable sensor compensation, 1 means enable is initialized, 0 means not initialized.</param>
        /// <param name="compliance_type">0 for not using any kind of compliance control method 1 for constant force compliance control, 2 for speed compliance control.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_compliant_type", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_compliant_type(ref int handle, ref int sensor_compensation, ref int compliance_type);

        /// <summary>
        /// Get admitrance control configurations.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="admit_ctrl_cfg">Pointer to JKTYPE.RobotAdmitCtrl return variable.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_admit_ctrl_config", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_admit_ctrl_config(ref int handle, ref JKTYPE.RobotAdmitCtrl admit_ctrl_cfg);

        /// <summary>
        /// Set the parameters for velocity complianance control.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="vel">Parameters for velocity compliance control.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_vel_compliant_ctrl", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_vel_compliant_ctrl(ref int handle, ref JKTYPE.VelCom vel);

        /// <summary>
        /// Set condition for compliance control.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="ft">The max force, if over limit, the robot will stop movement.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_compliance_condition", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_compliance_condition(ref int handle, ref JKTYPE.FTxyz ft);

        /// <summary>
        /// Set the reaction behavior of the cobot when connection to SDK is lost.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="millisecond">Timeout of connection loss, unit: ms.</param>
        /// <param name="mnt">Reaction behavior type.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_network_exception_handle", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_network_exception_handle(ref int handle, float millisecond, JKTYPE.ProcessType mnt);

        /// <summary>
        /// Set time interval of getting data via port 10004.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_status_data_update_time_interval", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_status_data_update_time_interval(ref int handle, float millisecond);

        /// <summary>
		/// Get the Denavit–Hartenberg parameters of the cobot.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="offset">Pointer of a varible to save the returned Denavit–Hartenberg parameters.</param>
		/// <returns></returns>
		[DllImport("jakaAPI.dll", EntryPoint = "get_dh_param", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_dh_param(ref int handle, ref JKTYPE.DHParam offset);

        /// <summary>
        /// Set installation (or mounting) angle of the cobot.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="angleX">Rotation angle around the X-axis.</param>
        /// <param name="angleZ">Rotation angle around the Z-axis.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "set_installation_angle", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int set_installation_angle(ref int handle, double angleX, double angleZ);

        /// <summary>
        /// Get installation (or mounting) angle of the cobot.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="quat">Pointer for the returned result in quaternion.</param>
        /// <param name="appang">Pointer for the returned result in RPY.</param>
        /// <returns></returns>
        [DllImport("jakaAPI.dll", EntryPoint = "get_installation_angle", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_installation_angle(ref int handle, ref JKTYPE.Quaternion quat, ref JKTYPE.Rpy appang);
    }
}
