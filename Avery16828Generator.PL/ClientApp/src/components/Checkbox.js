import React from "react";

const Checkbox = ({ label, isSelected, onCheckboxChange }) => (
<div className="form-check">
    <label>
        <input type="checkbox" name={label} onChange={onCheckboxChange} checked={isSelected}  className="form-check-input" />
        {label}
    </label>
    </div>
);

export default Checkbox;
