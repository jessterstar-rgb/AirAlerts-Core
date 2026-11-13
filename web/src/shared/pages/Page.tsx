import { FunctionComponent, JSX } from "react";

type PageProps = {
  children: JSX.Element;
};

export const Page: FunctionComponent<PageProps> = ({ children }) => {
  return (
    <>
      <header>Header</header>
      <section>{children}</section>
      <footer>Footer</footer>
    </>
  );
};
