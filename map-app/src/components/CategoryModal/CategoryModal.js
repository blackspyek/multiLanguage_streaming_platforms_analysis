import React, { useState, useEffect } from "react";
import Modal from "react-modal";
import axios from "axios";
import classes from "./categorymodal.module.css";
function CategoryModal({ isOpen, onClose, onCategorySelect }) {
  const [categories, setCategories] = useState([]);

  const [selectedCategories, setSelectedCategories] = useState([]);
  const [newCategory, setNewCategory] = useState("");
  const handleCategoryClick = (category) => {
    if (selectedCategories.includes(category)) {
      setSelectedCategories(selectedCategories.filter((c) => c !== category));
    } else {
      setSelectedCategories([...selectedCategories, category]);
    }
  };
  useEffect(() => {
    axios.get("http://localhost:5192/api/category").then((res) => {
      setCategories(res.data);
    });
  }, []);

  return (
    <Modal
      style={{
        content: {
          top: "50%",
          left: "50%",
          right: "auto",
          bottom: "auto",
          marginRight: "-50%",
          transform: "translate(-50%, -50%)",
          maxWidth: "70%",
        },
      }}
      isOpen={isOpen}
      onRequestClose={onClose}
      contentLabel="Category Modal"
    >
      <h1 className={classes.modalTitle}>Categories</h1>
      <div className={classes.container}>
        <div className={classes.categories}>
          {categories.map((category) => (
            <button
              key={category.id}
              onClick={() => handleCategoryClick(category.name)}
              className={`${classes.modalCategoryBtn} ${
                selectedCategories.includes(category.name)
                  ? classes.selected
                  : ""
              }`}
            >
              {category.name}
            </button>
          ))}
        </div>
        <div>
          <input
            type="text"
            className={classes.modalInputCategories}
            onChange={(e) => {
              setNewCategory(e.target.value);
            }}
            placeholder="Anime, Comedy"
          />
        </div>
      </div>
      <button
        className={classes.selectCategoriesBtn}
        onClick={() => {
          var selectedCategoriesVals =
            Object.values(selectedCategories).join(",");
          if (newCategory) {
            selectedCategoriesVals += "," + newCategory;
          }
          selectedCategoriesVals = selectedCategoriesVals.replace(/^,/, "");
          onCategorySelect(selectedCategoriesVals);
        }}
      >
        Select
      </button>
    </Modal>
  );
}

export default CategoryModal;
