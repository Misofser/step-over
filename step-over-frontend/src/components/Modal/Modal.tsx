import "./Modal.css"

type ModalProps = {
  title: string;
  children: React.ReactNode;
  onClose?: () => void;
};

export function Modal({ title, children, onClose }: ModalProps) {
  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div
        className="modal"
        onClick={(e) => e.stopPropagation()}
      >
        <h2>{title}</h2>
        {children}
      </div>
    </div>
  );
}
