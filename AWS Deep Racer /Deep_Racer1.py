def reward_function(params):
    '''
    Reward function for following the center line, staying on the track and reaching the goal
    '''
    
    # Read input parameters
    track_width = params['track_width']
    distance_from_center = params['distance_from_center']
    all_wheels_on_track = params['all_wheels_on_track']
    progress = params['progress']
    steps = params['steps']
    speed = params['speed']
    
    # Define markers for distance from center line and for progress along the track
    marker_1 = 0.1 * track_width
    marker_2 = 0.25 * track_width
    marker_3 = 0.5 * track_width
    progress_threshold_1 = 50.0
    progress_threshold_2 = 75.0
    
    # Give higher reward for being closer to the center line and for higher speed
    if distance_from_center <= marker_1:
        reward = 1.0
    elif distance_from_center <= marker_2:
        reward = 0.5
    elif distance_from_center <= marker_3:
        reward = 0.1
    else:
        reward = 1e-3  # likely crashed/ close to off track
        
    # Penalize the agent for leaving the track or driving slowly
    if not all_wheels_on_track:
        reward = 1e-3
    elif speed < 1.0:
        reward *= 0.5
    
    # Give additional rewards for making progress along the track and reaching the goal
    if progress > progress_threshold_2:
        reward += 10.0
    elif progress > progress_threshold_1:
        reward += 5.0
    elif progress > 0:
        reward += 1.0
    
    # Penalize the agent for taking too many steps to reach the goal
    if progress == 100.0 and steps > 0:
        reward += (1.0 / steps)
    
    return float(reward)
