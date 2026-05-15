export const metadata = {
  title: "ViSNET XR Backend",
  description: "Serverless API for the ViSNET Meta Quest XR assignment"
};

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
