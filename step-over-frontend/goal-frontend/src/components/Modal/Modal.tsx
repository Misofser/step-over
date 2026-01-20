import "./Modal.css"

type ModalProps = {
  title: string;
  children: React.ReactNode;
};

export function Modal({ title, children }: ModalProps) {
  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>{title}</h2>
        {children}
      </div>
    </div>
  );
}
