export default function Home() {
  return (
    <main style={{ fontFamily: "Arial, sans-serif", padding: 32, lineHeight: 1.5 }}>
      <h1>ViSNET XR Backend</h1>
      <p>Serverless API for the Meta Quest XR assignment.</p>
      <ul>
        <li><code>POST /api/login</code></li>
        <li><code>GET /api/projects</code></li>
        <li><code>GET /api/projects/:id/floors</code></li>
      </ul>
    </main>
  );
}
