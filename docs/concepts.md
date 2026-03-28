---
layout: default
title: Concepts
---

<section id="concepts">
    <h2>🧠 Core Concepts</h2>
    <h3>Rotation-based simulation</h3>
    <ul>
        <li>Riders rotate through lead, draft, and last-wheel positions</li>
        <li>Each rider has:
            <ul>
                <li>A pull duration</li>
                <li>A power target per position (1st, 2nd, 3rd, 4th+)</li>
            </ul>
        </li>
        <li>When a rider finishes their pull:
            <ul>
                <li>They rotate to the back</li>
                <li>Other riders move up one position</li>
            </ul>
        </li>
        <li>The pull leader defines the duration of the block for all riders</li>
    </ul>
    <p>This creates a realistic, repeatable simulation of a TTT paceline.</p>
</section>