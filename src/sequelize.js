import pkg from 'sequelize';
const { Sequelize, DataTypes } = pkg;

const sequelize = new Sequelize("laravel", "admin",  "go17032004", {
    host: 'localhost',
    dialect: "mysql",
    logging: false
  })


// sequelize.define("Coor", {
//     id: {
//         type: DataTypes.INTEGER,
//         autoIncrement: true,
//         primaryKey: true
//     },
//     lat: {
//         type: DataTypes.INTEGER,
//     },
//     lon: {
//         type: DataTypes.INTEGER,
//     },
//     number: {
//         type: DataTypes.STRING,
//         allowNull: true,
//         defaultValue: null
//     },
//     user_id: {
//         type: DataTypes.INTEGER,
//         primaryKey: true
//     }
    
// }, {
//     timestamps: true,
//     tableName: "coor_users"
// })


sequelize.sync({ alter: true });

export default sequelize;