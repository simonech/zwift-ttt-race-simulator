# Content Vision & Strategy

This document outlines the content strategy for the Zwift TTT Race Simulator project, defining what type of content should be published in different locations and for different audiences.

---

## 📝 README.md

**Audience:** Humans browsing the GitHub repository (developers, coaches, teams)

**Purpose:** Quick start guide and project overview

**Content Focus:**
- Concise project description and motivation (2-3 paragraphs)
- Link to full documentation website
- Quick start instructions (installation and basic usage)
- Minimal examples showing input/output
- Essential technical notes (e.g., System.CommandLine version pinning)
- Links to contributing guidelines and license

**What to KEEP:**
- Banner image
- High-level motivation
- Core concepts summary
- Basic CSV input/output examples
- CLI usage with short examples
- Key technical warnings (System.CommandLine)
- Development commands (build, test, run)
- License and disclaimer

**What to MOVE to Website (future):**
- Detailed roadmap items
- Extended motivation and use cases
- Comprehensive feature documentation
- Detailed troubleshooting guides
- Advanced usage scenarios
- In-depth technical architecture

**Style:**
- Scannable with clear headers
- Concise paragraphs (3-5 lines max)
- Focus on "getting started quickly"
- Link to website for deeper content

---

## 🌐 GitHub Pages Website (docs/)

**Audience:** End users, coaches, teams, and anyone wanting comprehensive documentation

**Purpose:** Complete, user-friendly documentation hub

**Content Focus:**
- Full feature documentation
- Detailed tutorials and guides
- Visual examples and screenshots
- Comprehensive roadmap
- Use case studies and examples
- FAQ and troubleshooting
- API reference (if applicable)
- Interactive examples (future)

**Current State:**
- Currently mirrors README.md content
- Static HTML with custom styling
- Includes banner and visualization examples

**Future Evolution:**
- More visual content (diagrams, videos, interactive demos)
- Dedicated pages for different sections (Getting Started, Advanced Usage, API Reference)
- User testimonials and real-world examples
- Community contributions showcase
- Tutorial series for coaches
- Integration guides with other tools
- Performance optimization tips

**Style:**
- Professional, polished presentation
- Rich media (images, diagrams, possibly videos)
- Navigation-friendly structure
- Mobile-responsive design
- SEO-optimized for discoverability

---

## 🤖 .github/copilot-instructions.md

**Audience:** AI coding assistants (GitHub Copilot, other AI agents)

**Purpose:** Provide context for AI-powered code generation and review

**Content Focus:**
- Project architecture and design patterns
- Coding standards and conventions
- Critical implementation details
- Testing requirements and patterns
- Build and development workflows
- Known gotchas and constraints
- Package version constraints (e.g., System.CommandLine)

**What to KEEP:**
- Architecture overview with data flow
- Key component descriptions
- Critical patterns (position-based power assignment)
- Coding standards (naming, structure, documentation)
- Testing guidelines (TDD approach, xUnit)
- Error handling patterns
- Development commands
- Technical constraints and warnings

**What to ADD (future):**
- Common refactoring patterns
- Performance optimization guidelines
- Security considerations
- Integration test patterns
- Code review checklist items
- Migration guides for breaking changes

**Style:**
- Technical and precise
- Structured for AI parsing
- Example-driven explanations
- Clear "do" and "don't" sections
- Links to relevant test files as examples

---

## 🎯 Content Principles

### README.md
- **Think:** "I want to understand this project in 2 minutes"
- **Goal:** Get someone from zero to running the tool quickly
- **Length:** Aim for <500 lines, link to website for details

### GitHub Pages
- **Think:** "I want to master this tool and understand all its capabilities"
- **Goal:** Comprehensive resource for all users
- **Length:** No limit, organize into logical sections/pages

### copilot-instructions.md
- **Think:** "I need to write code that fits this project's patterns"
- **Goal:** Enable AI to generate consistent, correct code
- **Length:** Focus on architecture, patterns, and critical details

---

## 🔄 Maintenance Strategy

### When to Update Each Location

**README.md:**
- Breaking changes to CLI or core workflow
- New installation requirements
- Critical warnings or notices
- License changes

**GitHub Pages:**
- New features or capabilities
- Detailed tutorials and guides
- Community examples
- FAQ updates
- Roadmap progress

**copilot-instructions.md:**
- Architecture changes
- New coding patterns or standards
- Build/test workflow changes
- Critical bug patterns to avoid
- New dependencies or version constraints

---

## 📊 Future Considerations

As the project grows:

1. **README.md** should become more focused, potentially just:
   - One-paragraph description
   - Installation command
   - Single quick-start example
   - Link to website for everything else

2. **GitHub Pages** should expand to include:
   - Multi-page documentation site (possibly using Jekyll or Docusaurus)
   - Interactive examples or embedded demos
   - Community gallery of race simulations
   - Video tutorials
   - Blog posts about TTT strategy

3. **copilot-instructions.md** should evolve with:
   - More detailed architecture diagrams
   - Performance benchmarking guidelines
   - Advanced testing patterns
   - Plugin/extension guidelines (if applicable)

---

## 📧 Feedback

This vision document should be treated as a living document. As the project evolves and community feedback is received, update this document to reflect the current strategy.
