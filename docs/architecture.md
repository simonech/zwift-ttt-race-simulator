---
layout: default
title: Architecture
---


        <section id="architecture">
            <h2>🏗️ Architecture</h2>
            <p>The simulator uses a layered architecture with clear separation of concerns:</p>
            <pre><code>CSV Input → CsvParser → PacelinePlanComposer → WorkoutProjector → ZwoExporter / ImageExporter</code></pre>

            <h3>Key Components</h3>
            <ul>
                <li><strong>Model Layer</strong> - Domain models representing the race structure:
                    <ul>
                        <li><code>PacelinePlan</code>: Complete race plan with all pulls and total duration</li>
                        <li><code>Pull</code>: One complete rotation cycle</li>
                        <li><code>PacelinePosition</code>: A rider's state within a pull</li>
                        <li><code>RiderPowerPlan</code>: Rider data with pull duration and position-based power values</li>
                        <li><code>WorkoutStep</code>: Duration + power pair for export</li>
                    </ul>
                </li>
                <li><strong>Services Layer</strong> - Business logic:
                    <ul>
                        <li><code>PacelinePlanComposer</code>: Orchestrates rotation sequence and generates the complete race plan</li>
                        <li><code>WorkoutProjector</code>: Transforms race plans into rider-specific workout steps</li>
                        <li><code>ParsedModelValidator</code>: Validates parsed rider models before plan composition</li>
                        <li><code>CsvParser</code>: Parses rider input CSV files</li>
                    </ul>
                </li>
                <li><strong>Exporters Layer</strong> - Output generation:
                    <ul>
                        <li><code>ZwoExporter</code>: Generates Zwift-compatible XML workout files</li>
                        <li><code>ImageExporter</code>: Creates PNG visualizations of power profiles</li>
                    </ul>
                </li>
            </ul>

            <h3>Design Pattern</h3>
            <p><strong>One Pull = One Complete Rotation Cycle</strong></p>
            <ul>
                <li>Each <code>Pull</code> represents all riders in their current positions</li>
                <li>Position-based power is determined by <code>RiderPowerPlan.GetPowerByPosition()</code></li>
                <li>Positions 4 and above reuse the last defined power value (index 3); this allows any team size to use the same 4-position power definition (position clamping)</li>
                <li>The pulling rider's pull duration determines the block duration for all riders</li>
            </ul>
        </section>

        <section id="development">
            <h2>🧪 Development Approach</h2>
            <p>This project is developed using <strong>Test-Driven Development (TDD)</strong>:</p>
            <ul>
                <li>Rotation logic is covered by unit tests</li>
                <li>Example-based tests validate known TTT scenarios</li>
                <li>Future extensions are added without breaking existing behavior</li>
            </ul>
            <p>This ensures the simulator remains predictable and safe to extend.</p>
        </section>