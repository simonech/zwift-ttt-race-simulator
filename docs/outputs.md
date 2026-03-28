---
layout: default
title: Outputs
---


        <section id="outputs">
            <h2>📤 Outputs</h2>
            <ul>
                <li>One structured workout per rider</li>
                <li>Exported as XML-based workout files (e.g. Zwift <code>.zwo</code> / WKO-compatible formats)</li>
                <li>Visual representation (PNG images) showing power profile over time with color-coded position indicators</li>
                <li>Workouts can be executed on:
                    <ul>
                        <li>Zwift</li>
                        <li>TrainerRoad</li>
                        <li>Wahoo SYSTM</li>
                        <li>Any platform supporting ERG mode workouts</li>
                    </ul>
                </li>
            </ul>

            <h3>Example Workout Visualizations</h3>
            
            <h4>Alice's Workout - Showcases Anaerobic, Tempo, Threshold and VO2Max zones:</h4>
            <img src="{{ '/assets/images/sample_alice.png' | relative_url }}" alt="Alice Workout Visualization" class="visualization">
            
            
            <h4>Bob's Workout - Showcases Threshold, VO2 Max, Recovery, and Endurance zones:</h4>
            <img src="{{ '/assets/images/sample_bob.png' | relative_url }}" alt="Bob Workout Visualization" class="visualization">
            
            <p>The visualizations show:</p>
            <ul>
                <li><strong>X-axis</strong>: Time (proportional to interval duration)</li>
                <li><strong>Y-axis</strong>: Power output in watts</li>
                <li><strong>FTP Line</strong>: White dotted horizontal line showing the rider's Functional Threshold Power</li>
                <li><strong>Color coding</strong> (based on intensity relative to FTP):
                    <ul>
                        <li>🔴 Red: Anaerobic (≥ 1.18 × FTP)</li>
                        <li>🟠 Orange: VO2 Max (≥ 1.05 × FTP)</li>
                        <li>🟡 Yellow: Threshold (≥ 0.90 × FTP)</li>
                        <li>🟢 Green: Tempo (≥ 0.75 × FTP)</li>
                        <li>🔵 Blue: Endurance (≥ 0.60 × FTP)</li>
                        <li>⚫ Gray: Recovery (&lt; 0.60 × FTP)</li>
                    </ul>
                </li>
            </ul>
        </section>